using IMitto.Net;
using IMitto.Pipelines;
using Nerdbank.Streams;
using System.Buffers;
using System.Net.Mime;
using System.Text;

namespace IMitto.Protocols;

public abstract class MittoTransportBase : IMittoTransport
{
	public abstract MittoProtocolVersion ProtocolVersion { get; }

	public virtual async Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token = default)
	{
		await WriteCommandAsync(writer, package.Command, token);
		await WriteHeadersAsync(writer, package.Headers, token);
		await WriteContentAsync(writer, package.Content, token);
	}

	public abstract Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default);

	public abstract Task<IMittoPackage> ReceiveAsync(ConnectionContext context, CancellationToken token = default);

	public abstract Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default);

	protected static string? ReadMittoHeaderKey(ref SequenceReader<byte> sequenceReader, int currentHeaderIndex)
	{
		if (!sequenceReader.TryRead(out byte keyId))
		{
			return null;
		}

		string? key;

		if (keyId == (byte)MittoHeaderKey.Custom)
		{
			if (!sequenceReader.TryRead(out byte keyLength) || !sequenceReader.TryReadExact(keyLength, out var keySpan))
			{
				return null;
			}

			key = Encoding.UTF8.GetString(keySpan);

			if (key?.StartsWith("unregistered-X") == false)
			{
				key = $"unregistered-x-{key}-{currentHeaderIndex + 1}-x-key";
			}
		}
		else
		{
			key = MittoHeaderKeys.HeaderIdToKey.TryGetValue((MittoHeaderKey)keyId, out var knownKey) ? knownKey : null;
		}

		return key ?? $"unregistered-{keyId}-{currentHeaderIndex + 1}-key";
	}

	protected virtual async Task<TCommand> ReadCommandAsync<TCommand>(MittoPipeReader reader, Func<MittoProtocolVersion, MittoAction, MittoModifier, TCommand> factory, CancellationToken token) where TCommand : class, IMittoCommand
	{
		TCommand? command = null;

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);

			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				while (sequenceReader.Remaining > 0 && command is null)
				{
					command = ReadCommand(factory, ref sequenceReader);
				}

				if (result.IsCompleted) { break; }

				return command!;
			}
			catch (Exception e)
			{
				// TODO: Handle exception
				reader.AdvanceTo(buffer.Start, buffer.End);
				break;
			}
			finally
			{
				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return command;
	}

	protected virtual async Task<THeaders> ReadHeadersAsync<THeaders>(MittoPipeReader reader, Func<THeaders> factory, Func<string, ReadOnlySequence<byte>, IMittoHeader> headerFactory, CancellationToken token = default) where THeaders : class, IMittoHeaders
	{
		var headers = factory();

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);

			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				if (!sequenceReader.TryReadExact(THeaders.HeaderCountLength, out var headerCountBytes)) { break; }

				var headerCount = MittoConverter.ToByte(headerCountBytes.FirstSpan);

				if (headerCount == 0) { break; }

				var currentHeaderIndex = 0;

				while (sequenceReader.Remaining > 0 && headerCount > currentHeaderIndex)
				{
					string? key = ReadMittoHeaderKey(ref sequenceReader, currentHeaderIndex);

					if (key is null) { break; }

					if (!sequenceReader.TryRead(out byte valueLength) || !sequenceReader.TryReadExact(valueLength, out var valueSpan)) { break; }

					var header = headerFactory(key, valueSpan);

					currentHeaderIndex++;

					headers.Add(header);
				}

				return headers;
			}
			catch (Exception e)
			{
				reader.AdvanceTo(buffer.Start, buffer.End);
				break;
			}
			finally
			{
				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return headers;
	}

	protected virtual async Task<TContent> ReadContentAsync<TContent>(MittoPipeReader reader, Func<ReadOnlySequence<byte>, TContent> factory, CancellationToken token) where TContent : class, IMittoContent
	{
		var sequence = new Sequence<byte>();
		var contentTotalLength = -1;
		var contentReadLength = 0;
		var remainingBytes = 0;

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);
			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				if (sequenceReader.Remaining > 0)
				{
					if (contentTotalLength < 0)
					{
						if (!sequenceReader.TryReadExact(TContent.ContentLengthByteCount, out var contentLengthBytes)) { break; }
						contentTotalLength = BitConverter.ToInt32(contentLengthBytes.FirstSpan);
						contentReadLength = 0;
						remainingBytes = contentTotalLength;
					}

					if (remainingBytes > 0)
					{
						if (remainingBytes >= sequenceReader.Remaining && sequenceReader.TryReadExact((int)sequenceReader.Remaining, out var contentBytes))
						{
							sequence.Write(contentBytes);
							contentReadLength += (int)contentBytes.Length;
							remainingBytes -= (int)contentBytes.Length;
						}
						else
						{
							sequenceReader.TryReadExact(remainingBytes, out var remainingContentBytes);
							sequence.Write(remainingContentBytes);
							contentReadLength += (int)remainingContentBytes.Length;
							remainingBytes -= (int)remainingContentBytes.Length;
						}
					}
				}

				if (contentTotalLength == contentReadLength)
				{
					return factory(sequence.AsReadOnlySequence);
				}

				if (result.IsCompleted) { break; }
			}
			catch (Exception e)
			{
				//_logger.LogError(e, "Error reading content");
				reader.AdvanceTo(buffer.Start, buffer.End);
			}
			finally
			{
				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return factory(ReadOnlySequence<byte>.Empty);
	}

	protected virtual async Task WriteCommandAsync(MittoPipeWriter writer, IMittoCommand command, CancellationToken token = default)
	{
		var buffer = writer.GetMemory(4096);
		var memoryWriter = new MemoryWriter<byte>(buffer);

		memoryWriter.Write((byte)command.Version);
		memoryWriter.Write(BitConverter.GetBytes((short)command.Action));
		memoryWriter.Write((byte)command.Modifier);

		writer.Advance(memoryWriter.WrittenLength);

		var flushResult = await writer.FlushAsync(token).Await();

		if (flushResult.IsCanceled)
		{
			throw new OperationCanceledException("Write command operation was canceled.");
		}

		if (flushResult.IsCompleted)
		{
			throw new InvalidOperationException("Write command operation was completed.");
		}
	}

	protected virtual async Task WriteHeadersAsync(MittoPipeWriter writer, IMittoHeaders headers, CancellationToken token = default)
	{
		var buffer = new AutoAdvancingBufferWriter<byte>(writer);

		buffer.Write((byte)headers.Count);

		foreach (var kvp in headers)
		{
			if (!kvp.HasStringKey)
			{
				buffer.Write((byte)kvp.KeyId);
			}
			else
			{
				buffer.Write((byte)MittoHeaderKey.Custom);
				var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
				buffer.Write((byte)keyBytes.Length);
				buffer.Write(keyBytes);
			}

			if (kvp.IsSerialized)
			{
				buffer.Write(kvp.Content);
			}
			else
			{
				var valueBytes = Encoding.UTF8.GetBytes(kvp.Value);

				if (valueBytes.Length > byte.MaxValue)
				{
					throw new InvalidDataException($"Header value is too long: {kvp.Key}:{kvp.Value}");
				}

				buffer.Write((byte)valueBytes.Length);
				buffer.Write(valueBytes);
			}
		}

		writer.Advance(buffer.WrittenLength);

		var flushResult = await writer.FlushAsync(token).Await();

		if (flushResult.IsCanceled)
		{
			throw new OperationCanceledException("Write command operation was canceled.");
		}

		if (flushResult.IsCompleted)
		{
			throw new InvalidOperationException("Write command operation was completed.");
		}
	}

	protected virtual async Task WriteContentAsync(MittoPipeWriter writer, IMittoContent content, CancellationToken token = default)
	{
		var contentBytes = content.Content;
		var contentLengthInBytes = Array.Empty<byte>();

		if (content.IsSerialized)
		{
			contentLengthInBytes = BitConverter.GetBytes((int)content.Content.Length);
		}
		else
		{
			contentBytes = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(content.Package));
			contentLengthInBytes = BitConverter.GetBytes((int)contentBytes.Length);
		}

		if (contentBytes.Length >= writer.MinimumWriteSize)
		{
			var buffer = new AutoAdvancingBufferWriter<byte>(writer);
			buffer.Write(contentLengthInBytes);
			buffer.Write(contentBytes);
		}
		else
		{
			writer.Write(contentLengthInBytes);
			writer.Write(contentBytes);
		}

		writer.Advance((int)contentBytes.Length);

		var flushResult = await writer.FlushAsync(token).Await();

		if (flushResult.IsCanceled)
		{
			throw new OperationCanceledException("Write command operation was canceled.");
		}

		if (flushResult.IsCompleted)
		{
			throw new InvalidOperationException("Write command operation was completed.");
		}
	}

	protected abstract IMittoPackage CreatePackage(IMittoCommand command, IMittoHeaders headers, IMittoContent content);

	private static TCommand? ReadCommand<TCommand>(Func<MittoProtocolVersion, MittoAction, MittoModifier, TCommand> factory, ref SequenceReader<byte> sequenceReader)
		where TCommand : class, IMittoCommand
	{
		TCommand? command = null;

		if (!sequenceReader.TryReadExact(TCommand.VersionLength, out var versionBytes))
		{
			return null;
		}

		MittoProtocolVersion version = MittoConverter.ToProtocolVersion(versionBytes.FirstSpan);

		if (!sequenceReader.TryReadExact(TCommand.ActionLength, out var actionBytes))
		{
			return null;
		}

		var action = MittoConverter.ToAction(actionBytes.FirstSpan);

		if (!sequenceReader.TryReadExact(TCommand.ModifierLength, out var modifierBytes))
		{
			return null;
		}

		var modifier = MittoConverter.ToModifier(modifierBytes.FirstSpan);

		command = factory(version, action, modifier);

		return command;
	}
}
