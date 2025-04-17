using IMitto.Net;
using IMitto.Pipelines;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Text;

namespace IMitto.Protocols.V1;

internal class MittoTransport : MittoTransportBase
{
	public override MittoProtocolVersion ProtocolVersion => MittoProtocolVersion.V1;

	public override async Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default)
	{
		var command = await ReadCommandAsync(reader, token);
		var header = await ReadHeadersAsync(reader, token);
		var content = await ReadContentAsync(reader, token);

		return new MittoPackage(command, header, content);
	}

	public override Task<IMittoPackage> ReceiveAsync(ConnectionContext context, CancellationToken token)
	{
		var reader = context.Socket.GetReader();
		return ReadPackageAsync(reader, token);
	}

	public override Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token)
	{
		var writer = context.Socket.GetWriter();
		return WritePackageAsync(writer, package, token);
	}

	public override async Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token = default)
	{
		await WriteCommandAsync(writer, package.Command, token);
		await WriteHeadersAsync(writer, package.Headers, token);
		await WriteContentAsync(writer, package.Content, token);
	}

	private static async Task<IMittoCommand> ReadCommandAsync(MittoPipeReader reader, CancellationToken token)
	{
		IMittoCommand? command = null;

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);

			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				while (sequenceReader.Remaining > 0 && command is null)
				{
					if (!sequenceReader.TryReadExact(MittoCommand.VersionLength, out var versionBytes)) { break; }

					var version = MittoConverter.ToProtocolVersion(versionBytes.FirstSpan);

					if (!sequenceReader.TryReadExact(MittoCommand.ActionLength, out var actionBytes)) { break; }

					var action = MittoConverter.ToAction(actionBytes.FirstSpan);

					if (!sequenceReader.TryReadExact(MittoCommand.ModifierLength, out var modifierBytes)) { break; }

					command = new MittoCommand(version, action, MittoConverter.ToModifier(modifierBytes.FirstSpan));

					return command;
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

		return command;
	}

	private static async Task<IMittoHeaders> ReadHeadersAsync(MittoPipeReader reader, CancellationToken token = default)
	{
		var headers = new MittoHeaders();

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);

			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				if (!sequenceReader.TryReadExact(1, out var headerCountBytes)) { break; }

				var headerCount = MittoConverter.ToByte(headerCountBytes.FirstSpan);

				if (headerCount == 0) { break; }

				var currentHeaderIndex = 0;

				while (sequenceReader.Remaining > 0 && headerCount > currentHeaderIndex)
				{
					if (!sequenceReader.TryRead(out byte keyId)) { break; }

					string? key;

					if (keyId == (byte)MittoHeaderKey.Custom)
					{
						if (!sequenceReader.TryRead(out byte keyLength) || !sequenceReader.TryReadExact(keyLength, out var keySpan)) { break; }

						key = Encoding.UTF8.GetString(keySpan);

						if (key?.StartsWith("unregistered-") == false)
						{
							key = $"unregistered-{key}-{currentHeaderIndex + 1}-key";
						}
					}
					else
					{
						key = MittoHeaderKeys.HeaderIdToKey.TryGetValue((MittoHeaderKey)keyId, out var knownKey) ? knownKey : null;
					}

					if (string.IsNullOrEmpty(key))
					{
						key = $"unknown-{keyId}-{currentHeaderIndex + 1}";
					}

					if (!sequenceReader.TryRead(out byte valueLength) || !sequenceReader.TryReadExact(valueLength, out var valueSpan)) { break; }

					var header = new MittoHeader(key, valueSpan);
					
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

	private static async Task<MittoContent> ReadContentAsync(MittoPipeReader reader, CancellationToken token)
	{
		MittoContent? content = null;

		while (!token.IsCancellationRequested)
		{
			var result = await reader.ReadAsync(token);

			var buffer = result.Buffer;
			var sequenceReader = new SequenceReader<byte>(buffer);

			try
			{
				while (sequenceReader.Remaining > 0 && content is null)
				{
					if (!sequenceReader.TryReadExact(MittoContent.ContentLengthByteCount, out var contentLengthBytes)) { break; }

					var contentLength = BitConverter.ToInt32(contentLengthBytes.FirstSpan);

					if (!sequenceReader.TryReadExact(contentLength, out var contentBytes)) { break; }

					content = new MittoContent(contentBytes);
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

	private static async Task WriteCommandAsync(MittoPipeWriter writer, IMittoCommand command, CancellationToken token = default)
	{
		await Task.Delay(1, token);

		var buffer = writer.GetMemory(4096);
		var memoryWriter = new MemoryWriter<byte>(buffer);

		memoryWriter.Write((byte)command.Version);
		memoryWriter.Write(BitConverter.GetBytes((short)command.Action));
		memoryWriter.Write((byte)command.Modifier);

		//writer.WriteAsync.Write(memoryWriter.WrittenSpan, token).Await();

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

	private static async Task WriteHeadersAsync(MittoPipeWriter writer, IMittoHeaders headers, CancellationToken token = default)
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

	private static async Task WriteContentAsync(MittoPipeWriter writer, IMittoContent content, CancellationToken token = default)
	{
		await Task.Delay(1, token);

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
}
