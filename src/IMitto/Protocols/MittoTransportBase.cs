using IMitto.Net;
using IMitto.Pipelines;
using System.Buffers;
using System.Text;

namespace IMitto.Protocols;

public abstract class MittoTransportBase : IMittoTransport
{
	public static readonly MittoProtocolVersion Version = MittoProtocolVersion.V1;

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

	protected virtual async Task<TCommand> ReadCommandAsync<TCommand>(MittoPipeReader reader, Func<MittoAction, MittoModifier, TCommand> factory, CancellationToken token) where TCommand : class, IMittoCommand
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
					command = ReadMittoCommand(factory, ref sequenceReader);
				}

				if (result.IsCompleted) { break; }

				return command;
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

		return command;
	}

	private static TCommand? ReadMittoCommand<TCommand>(Func<MittoAction, MittoModifier, TCommand> factory, ref SequenceReader<byte> sequenceReader) where TCommand : class, IMittoCommand
	{
		TCommand? command = null;

		if (!sequenceReader.TryReadExact(TCommand.VersionLength, out var versionBytes))
		{
			return null;
		}

		var version = MittoConverter.ToProtocolVersion(versionBytes.FirstSpan);

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

		command = factory(action, modifier);

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

				if (result.IsCompleted) { break; }
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
		TContent? content = null;

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);

			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				while (sequenceReader.Remaining > 0 && content is null)
				{
					if (!sequenceReader.TryReadExact(TContent.ContentLengthByteCount, out var contentLengthBytes)) { break; }

					var contentLength = BitConverter.ToInt32(contentLengthBytes.FirstSpan);

					if (!sequenceReader.TryReadExact(contentLength, out var contentBytes)) { break; }

					content = factory(contentBytes);
				}

				if (result.IsCompleted) { break; }
			}
			catch (Exception e)
			{
				//_logger.LogError(e, "Error reading content");
				reader.AdvanceTo(buffer.Start, buffer.End);
				break;
			}
			finally
			{
				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return content;
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
		await Task.Delay(1, token);

		var buffer = writer.GetMemory();
		var memoryWriter = new MemoryWriter<byte>(buffer);


		memoryWriter.Write((byte)headers.Count);

		foreach (var kvp in headers)
		{
			if (!kvp.HasStringKey)
			{
				memoryWriter.Write((byte)kvp.KeyId);
			}
			else
			{
				memoryWriter.Write((byte)MittoHeaderKey.Custom);
				var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
				memoryWriter.Write((byte)keyBytes.Length);
				memoryWriter.Write(keyBytes);
			}

			if (kvp.IsSerialized && kvp is IMittoByteContent<byte> content)
			{
				memoryWriter.Write(content.Content);
			}
			else
			{
				//var valueBytes = Encoding.UTF8.GetByteCount(kvp.Value);
				//memoryWriter.Write
				//Encoding.UTF8.GetChars(kvp.Value.AsSpan(), writer);

				var valueBytes = Encoding.UTF8.GetBytes(kvp.Value);
				memoryWriter.Write((byte)valueBytes.Length);
				memoryWriter.Write(valueBytes);
			}
		}

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

	protected virtual async Task WriteContentAsync(MittoPipeWriter writer, IMittoContent content, CancellationToken token = default)
	{
		var buffer = writer.GetMemory();
		var memoryWriter = new MemoryWriter<byte>(buffer);

		if (content.IsSerialized && content is IMittoByteContent<int> byteContent)
		{
			memoryWriter.Write(BitConverter.GetBytes((int)byteContent.Content.Length));
			memoryWriter.Write(byteContent.Content);
		}
		else
		{
			var contentBytes = Encoding.UTF8.GetBytes(content.Package!);
			memoryWriter.Write(BitConverter.GetBytes(contentBytes.Length));
			memoryWriter.Write(contentBytes);
		}

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

	protected abstract IMittoPackage CreatePackage(IMittoCommand command, IMittoHeaders headers, IMittoContent content);
}
