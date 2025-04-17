using IMitto.Net;
using IMitto.Pipelines;
using IMitto.Protocols.V1;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace IMitto.Protocols
{
	public abstract class MittoProtcolTransportBase : IProtocolTransport
	{
		public abstract MittoProtocolVersion ProtocolVersion { get; }

		public abstract Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default);

		public abstract Task<IMittoPackage> ReceiveAsync(ConnectionContext context, CancellationToken token = default);

		public abstract Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default);

		public abstract Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token = default);
	}
}

public static class Extensions
{
	private static bool TryReadExact(this ref SequenceReader<byte> reader, int length, out ReadOnlySpan<byte> span)
	{
		if (reader.Remaining < length)
		{
			span = default;
			return false;
		}

		span = reader.Sequence.Slice(reader.Position, length).ToArray();
		reader.Advance(length);
		return true;
	}
}


public static class HeaderSerializer
{
	public static byte[] Serialize(Dictionary<string, string> headers)
	{
		using var ms = new MemoryStream();

		foreach (var kvp in headers)
		{
			if (MittoHeaderKeys.KeyToHeaderId.TryGetValue(kvp.Key, out MittoHeaderKey keyId))
			{
				ms.WriteByte((byte)keyId);
				var valueBytes = Encoding.UTF8.GetBytes(kvp.Value);
				ms.WriteByte((byte)valueBytes.Length);
				ms.Write(valueBytes, 0, valueBytes.Length);
			}
			else
			{
				ms.WriteByte((byte)MittoHeaderKey.Custom);

				var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
				var valueBytes = Encoding.UTF8.GetBytes(kvp.Value);

				ms.WriteByte((byte)keyBytes.Length);
				ms.Write(keyBytes);
				ms.WriteByte((byte)valueBytes.Length);
				ms.Write(valueBytes);
			}
		}

		return ms.ToArray();
	}

	public static Dictionary<string, string> Deserialize(byte[] data)
	{
		var headers = new Dictionary<string, string>();

		using var ms = new MemoryStream(data);
		while (ms.Position < ms.Length)
		{
			var keyId = ms.ReadByte();
			if (keyId == -1) break;

			if (keyId != (byte)MittoHeaderKey.Custom)
			{
				if (!MittoHeaderKeys.HeaderIdToKey.TryGetValue((MittoHeaderKey)keyId, out var key))
					key = $"unknown-{keyId}";

				var valueLength = ms.ReadByte();
				if (valueLength == -1) break;

				var valueBytes = new byte[valueLength];
				ms.Read(valueBytes, 0, valueLength);
				var value = Encoding.UTF8.GetString(valueBytes);
				headers[key] = value;
			}
			else
			{
				var keyLength = ms.ReadByte();
				if (keyLength == -1) break;

				var keyBytes = new byte[keyLength];
				ms.Read(keyBytes, 0, keyLength);
				string key = Encoding.UTF8.GetString(keyBytes);

				int valueLength = ms.ReadByte();
				if (valueLength == -1) break;

				byte[] valueBytes = new byte[valueLength];
				ms.Read(valueBytes, 0, valueLength);
				var value = Encoding.UTF8.GetString(valueBytes);
				headers[key] = value;
			}
		}

		return headers;
	}
}
