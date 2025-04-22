using IMitto.Pipelines;
using IMitto.Protocols;
using IMitto.Settings;
using System.Buffers;
using System.Text;

namespace IMitto.Tests.Protocols;

public class ProtocolTransportTests
{

	[Theory]
	[InlineData(Data.Small, Data.SmallBody)]
	[InlineData(Data.Small, Data.MediumBody)]
	[InlineData(Data.Small, Data.LargeBody)]
	[InlineData(Data.Medium, Data.SmallBody)]
	[InlineData(Data.Medium, Data.MediumBody)]
	[InlineData(Data.Medium, Data.LargeBody)]
	[InlineData(Data.Large, Data.SmallBody)]
	[InlineData(Data.Large, Data.MediumBody)]
	[InlineData(Data.Large, Data.LargeBody)]
	[InlineData(Data.Large, Data.ExtraLarge)]
	public void TestingProtocolFormat(string headerKey, string body)
	{
		var kvpHeaders = Data.GetHeaders(headerKey);

		// 1st 2bytes/16bits
		var actionDescriptor = new byte[5];
		// 2nd 1byte/4bits

		// Action Descriptor
		byte version = (byte)MittoProtocolVersion.V1;
		short action = 0b0000_0100;
		byte modifier = 0b0000_0000;
		byte headerCound = (byte)kvpHeaders.Count;

		// Version
		actionDescriptor[0] = version;

		// Action
		byte[] actionBytes = BitConverter.GetBytes(action);
		actionDescriptor[1] = actionBytes[0];
		actionDescriptor[2] = actionBytes[1];

		// Modifier
		actionDescriptor[3] = modifier;

		// Header Count
		actionDescriptor[4] = headerCound;

		// 3rd 4bytes/int32 Header Length
		var headerStrings = kvpHeaders.Select(kvp => $"{kvp.Key}:{kvp.Value}");

		var headersBuffer = headerStrings.SelectMany<string, byte>(header => {
			var headerBytes = Encoding.UTF8.GetBytes(header)!;
			var headerLength = (byte)headerBytes.Length;
			return [headerLength, .. headerBytes];
		}).ToArray();
		var headersLength = headersBuffer.Length;

		// 4th 4bytes/int32 Body Content
		var bodyBytes = Encoding.UTF8.GetBytes(body);
		var bodyLength = bodyBytes.Length;

		// Write the content.
		var buffer = ArrayPool<byte>.Shared.Rent(13 + headersBuffer.Length + bodyLength);

		using var ms = new MemoryStream(buffer);
		// Descriptor
		ms.Write(actionDescriptor);

		// Headers
		ms.Write(headersBuffer);

		// Package length and content
		ms.Write(BitConverter.GetBytes(bodyLength));
		ms.Write(bodyBytes);

		ms.Position = 0;

		// Read the content.
		Span<byte> readDescriptor = stackalloc byte[5];
		ms.ReadExactly(readDescriptor);

		Assert.Equal(5, readDescriptor.Length);
		Assert.Equal(actionDescriptor, readDescriptor);

		// Read the action
		var readVersion = readDescriptor[0];
		readDescriptor = readDescriptor.Slice(1);

		var actualAction = (MittoAction)BitConverter.ToInt16(readDescriptor[0..2]);

		readDescriptor = readDescriptor.Slice(2);

		Assert.Equal((short)MittoAction.Connect, action);
		Assert.Equal(MittoAction.Connect, actualAction);

		// Read the modifier
		var actualActionModifier = (MittoModifier)readDescriptor[0];

		readDescriptor = readDescriptor.Slice(1);

		// Read the modifier
		var actualHeaderCount = readDescriptor[0];

		Assert.Equal((short)MittoAction.Connect, action);
		Assert.Equal(MittoModifier.None, actualActionModifier);

		var readHeaders = new Dictionary<string, string>(headerCound);
		var readHeadersLength = 0;

		for (var i = 0; i < actualHeaderCount; i++)
		{
			var headerLength = ms.ReadByte();
			var headerBuffer = new byte[headerLength];

			ms.ReadExactly(headerBuffer);

			var header = Encoding.UTF8.GetString(headerBuffer);
			var kvp = header.Split(':');

			readHeaders.Add(kvp[0], kvp[1]);
			readHeadersLength += 1 + headerBuffer.Length;
		}

		Assert.Equal(headersLength, readHeadersLength);
		Assert.Equal(kvpHeaders, readHeaders);

		// Read body length
		var readBodyLength = 0;
		var readBodyBuffer = new byte[4];
		ms.ReadExactly(readBodyBuffer, 0, 4);
		readBodyLength = BitConverter.ToInt32(readBodyBuffer);
		readBodyBuffer = new byte[readBodyLength];
		ms.ReadExactly(readBodyBuffer, 0, readBodyLength);

		Assert.Equal(bodyLength, readBodyLength);
		Assert.Equal(body, Encoding.UTF8.GetString(readBodyBuffer, 0, readBodyLength));
	}

	[Theory]
	[InlineData(Data.Small, Data.SmallBody)]
	[InlineData(Data.Small, Data.MediumBody)]
	[InlineData(Data.Small, Data.LargeBody)]
	[InlineData(Data.Medium, Data.SmallBody)]
	[InlineData(Data.Medium, Data.MediumBody)]
	[InlineData(Data.Medium, Data.LargeBody)]
	[InlineData(Data.Large, Data.SmallBody)]
	[InlineData(Data.Large, Data.MediumBody)]
	[InlineData(Data.Large, Data.LargeBody)]
	[InlineData(Data.Large, Data.ExtraLarge)]
	public void TestingProtocolFormatSerializingReading(string headerKey, string body)
	{
		var kvpHeaders = Data.GetHeaders(headerKey);

		// 1st 2bytes/16bits
		var actionDescriptor = new byte[4];

		// 2nd 1byte/4bits

		// Action Descriptor
		byte version = (byte)MittoProtocolVersion.V1;
		short action = 0b0000_0100;
		byte modifier = 0b0000_0000;
		byte headerCound = (byte)kvpHeaders.Count;

		// Version
		actionDescriptor[0] = version;

		// Action
		byte[] actionBytes = BitConverter.GetBytes(action);
		actionDescriptor[1] = actionBytes[0];
		actionDescriptor[2] = actionBytes[1];

		// Modifier
		actionDescriptor[3] = modifier;

		// 3rd 4bytes/int32 Header Length
		var headersList = new List<byte>
		{
			headerCound
		};

		foreach (var kvp in kvpHeaders)
		{
			if (MittoHeaderKeys.KeyToHeaderId.TryGetValue(kvp.Key, out var keyId))
			{
				headersList.Add((byte)keyId);
			}
			else
			{
				headersList.Add((byte)MittoHeaderKey.Custom);

				var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
				headersList.Add((byte)keyBytes.Length);
				headersList.AddRange(keyBytes);
			}
			var valueBytes = Encoding.UTF8.GetBytes(kvp.Value);
			headersList.Add((byte)valueBytes.Length);
			headersList.AddRange(valueBytes);
		}

		// Header Count
		var headersBuffer = headersList.ToArray();
		var headersLength = headersBuffer.Length;

		// 4th 4bytes/int32 Body Content
		var bodyBytes = Encoding.UTF8.GetBytes(body);
		var bodyLength = bodyBytes.Length;

		// Write the content.
		var buffer = ArrayPool<byte>.Shared.Rent(10 + headersLength + bodyLength);

		using var ms = new MemoryStream(buffer);
		// Descriptor
		ms.Write(actionDescriptor);

		// Headers
		ms.Write(headersBuffer);

		// Package length and content
		ms.Write(BitConverter.GetBytes(bodyLength));
		ms.Write(bodyBytes);

		ms.Position = 0;

		var reader = MittoPipe.CreateReader(ms, MittoOptions.Default.Pipeline);
		var package = MittoProtocol.ReadPackageAsync(reader, MittoProtocolVersion.V1, CancellationToken.None);
	}

	private const int _iterations = 2;

	[Theory]
	[InlineData(Data.Small, Data.SmallBody)]
	[InlineData(Data.Small, Data.MediumBody)]
	[InlineData(Data.Small, Data.LargeBody)]
	[InlineData(Data.Medium, Data.SmallBody)]
	[InlineData(Data.Medium, Data.MediumBody)]
	[InlineData(Data.Medium, Data.LargeBody)]
	[InlineData(Data.Large, Data.SmallBody)]
	[InlineData(Data.Large, Data.MediumBody)]
	[InlineData(Data.Large, Data.LargeBody)]
	[InlineData(Data.Large, Data.ExtraLarge)]
	[InlineData(Data.Large, Data.XxLarge)]
	[InlineData("", "")]
	public async Task TestingProtocolFormatSerializingReadingAndWriting(string headerKey, string body)
	{
		var kvpHeaders = Data.GetHeaders(headerKey);

		var builder = MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Session)
			.AddHeaders(kvpHeaders)
			.WithModifier(MittoModifier.Start)
			.WithPackage(body switch
			{
				Data.ExtraLarge => Data.TenKb,
				Data.XxLarge => Data.TenMb,
				_ => body
			});

		var contentLength = Encoding.UTF8.GetByteCount(body switch
		{
			Data.ExtraLarge => Data.TenKb,
			Data.XxLarge => Data.TenMb,
			_ => body
		});

		var package = builder.Build();

		using var stream = new MemoryStream();

		for (var i = 0; i < _iterations; i++)
		{
			stream.Position = 0;

			var writer = MittoPipe.CreateWriter(stream, MittoOptions.Default.Pipeline);
			await MittoProtocol.WritePackageAsync(writer, package, CancellationToken.None);

			stream.Position = 0;

			var reader = MittoPipe.CreateReader(stream, MittoOptions.Default.Pipeline);
			package = await MittoProtocol.ReadPackageAsync(reader, MittoProtocolVersion.V1, CancellationToken.None);

			Assert.Equal(MittoAction.Session, package.Command.Action);
			Assert.Equal(MittoModifier.Start, package.Command.Modifier);
			Assert.Equal(MittoProtocolVersion.V1, package.Command.Version);
			Assert.True(package.Content.Content.Length == contentLength);

			// + 2 is from encoding and timestamp headers that are added
			Assert.Equal(kvpHeaders.Count + 2, package.Headers.Count);
		}
	}

	private static class Data
	{
		public const string Small = nameof(Small);
		public const string Medium = nameof(Medium);
		public const string Large = nameof(Large);
		public const string ExtraLarge = nameof(ExtraLarge);
		public const string XxLarge = nameof(XxLarge);

		private static readonly Dictionary<string, MittoHeader> Headers = new()
		{
			[Small] = new MittoHeader(SmallHeaders),
			[Medium] = new MittoHeader(MediumHeaders),
			[Large] = new MittoHeader(LargeHeaders)
		};

		public const string SmallHeaders = "encoding:value1\nkey2:value2";
		public const string SmallBody = "Lorem ipsum dolor sit amet, consectetur.";

		public const string MediumHeaders = "key1:value1\ncontent-type:value2\nkey3:value3\ncorrelation-id:value4\nkey5:value5";
		public const string MediumBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore.";

		public const string LargeHeaders = "timestamp:value1\ncorrelation-id:value2\nkey3:value3\nkey4:value4\nkey5:value5\nkey6:value6\nkey7:value7\nkey8:value8";
		public const string LargeBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

		public static readonly string TenKb = new('a', 1024 * 10); // 10KB

		public static readonly string TenMb = new('a', 1024 * 1024 * 10); // 10MB

		public static IDictionary<string, string> GetHeaders(string name) => Headers.ContainsKey(name) ? Headers[name].Headers : [];

		public sealed class MittoHeader(string headers)
		{
			public Dictionary<string, string> Headers { get; } = headers.Split('\n')
				.Select(x => x.Split(':'))
				.ToDictionary(x => x[0], x => x[1]);
		}
	}
}
