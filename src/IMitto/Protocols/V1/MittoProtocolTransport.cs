using IMitto.Net;
using IMitto.Pipelines;
using System;
using System.Buffers;
using System.Reflection.PortableExecutable;
using System.Text;

namespace IMitto.Protocols.V1;

internal class MittoProtocolTransport : MittoProtcolTransportBase
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
		throw new NotImplementedException();
	}

	public override Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token)
	{
		throw new NotImplementedException();
	}

	private async static Task<IMittoCommand> ReadCommandAsync(MittoPipeReader reader, CancellationToken token)
	{
		IMittoCommand? command = null;

		while (true)
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
			}
			finally
			{
				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return command;
	}

	private async static Task<IMittoHeaders> ReadHeadersAsync(MittoPipeReader reader, CancellationToken token = default)
	{
		var headers = new MittoHeaders();

		while (true)
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

					var header = new MittoHeader(key, (MittoHeaderKey)keyId, valueSpan);
					
					currentHeaderIndex++;

					headers.Add(header);
				}

				if (result.IsCompleted) { break; }
			}
			catch (Exception e)
			{
				reader.AdvanceTo(buffer.Start, buffer.End);
			}
			finally
			{
				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return headers;
	}

	private async static Task<MittoContent> ReadContentAsync(MittoPipeReader reader, CancellationToken token)
	{
		MittoContent? content = null;

		while (true)
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
			}
			finally
			{

				reader.AdvanceTo(sequenceReader.Position, buffer.End);
			}
		}

		return content;
	}

	public override async Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token = default)
	{
		await Task.Delay(1, token);

		//var buffer = writer.GetMemory();
		//var memoryWriter = new MemoryWriter<byte>(buffer);

		//foreach (var kvp in headers)
		//{
		//	if (!kvp.HasStringKey)
		//	{
		//		memoryWriter.Write((byte)kvp.KeyId);
		//	}
		//	else
		//	{
		//		memoryWriter.Write((byte)MittoHeaderKey.Custom);
		//		var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
		//		memoryWriter.Write((byte)keyBytes.Length);
		//		memoryWriter.WriteBytes(keyBytes);
		//	}
		//}

		//writer.Advance(memoryWriter.WrittenLength);

		//var flushResult = await writer.FlushAsync(token).Await();
	}
}
