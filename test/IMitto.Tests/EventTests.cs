using System.Buffers;
using System.Text;

namespace IMitto.Tests;

public class EventTests
{
	[Fact]
	public void EventSubscriptionTest()
	{
		using (var subscription = Subscribe.ToLocalEvent<string>("eventId", s => Assert.True(s.Data == "Event raised.")))
		{
			subscription.Publish("Event raised.");

			subscription.Unsubscribe();
		}
	}

	[Theory]
	[InlineData(Data.SmallHeaders, Data.SmallBody)]
	[InlineData(Data.SmallHeaders, Data.MediumBody)]
	[InlineData(Data.SmallHeaders, Data.LargeBody)]
	[InlineData(Data.MediumHeaders, Data.SmallBody)]
	[InlineData(Data.MediumHeaders, Data.MediumBody)]
	[InlineData(Data.MediumHeaders, Data.LargeBody)]
	[InlineData(Data.LargeHeaders, Data.SmallBody)]
	[InlineData(Data.LargeHeaders, Data.MediumBody)]
	[InlineData(Data.LargeHeaders, Data.LargeBody)]
	public void TestingProtocolFormat(string headers, string body)
	{
		// 1st 2bytes/16bits
		var actionDescriptor = new byte[3];
		// 2nd 1byte/4bits

		// Action Descriptor
		short action = 0b0100;
		byte modifier = 0b0000;

		// Action
		byte[] actionBytes = BitConverter.GetBytes(action);
		actionDescriptor[0] = actionBytes[0];
		actionDescriptor[1] = actionBytes[1];

		// Modifier
		actionDescriptor[2] = modifier;

		// 3rd 4bytes/int32 Header Length
		var headersBytes = Encoding.UTF8.GetBytes(headers);
		var headersLength = headersBytes.Length;

		// 4th 4bytes/int32 Body Content
		var bodyBytes = Encoding.UTF8.GetBytes(body);
		var bodyLength = bodyBytes.Length;

		// Write the content.
		var buffer = ArrayPool<byte>.Shared.Rent(11 + headersLength + bodyLength);

		using var ms = new MemoryStream(buffer);
		ms.Write(actionDescriptor);

		ms.Write(BitConverter.GetBytes(headersLength));
		ms.Write(headersBytes);

		ms.Write(BitConverter.GetBytes(bodyLength));
		ms.Write(bodyBytes);

		ms.Position = 0;

		// Read the content.
		var readActionDescriptor = new byte[3];
		var read = ms.Read(readActionDescriptor);

		Assert.Equal(3, read);
		Assert.Equal(actionDescriptor, readActionDescriptor);

		// Read the action
		var actualAction = (MittoAction)BitConverter.ToInt16(readActionDescriptor.Take(2).ToArray());

		Assert.Equal((short)MittoAction.Connect, action);
		Assert.Equal(MittoAction.Connect, actualAction);

		// Read the modifier
		var actualActionModifier = (MittoActionModifier)readActionDescriptor[2];

		Assert.Equal((short)MittoAction.Connect, action);
		Assert.Equal(MittoActionModifier.None, actualActionModifier);

		// Read header length
		var readHeadersLength = 0;
		var readHeadersBuffer = new byte[4];
		ms.ReadExactly(readHeadersBuffer, 0, 4);
		readHeadersLength = BitConverter.ToInt32(readHeadersBuffer);

		// Read the headers
		readHeadersBuffer = new byte[readHeadersLength];
		ms.ReadExactly(readHeadersBuffer, 0, readHeadersLength);
		var readHeaders = Encoding.UTF8.GetString(readHeadersBuffer);

		Assert.Equal(headersLength, readHeadersLength);
		Assert.Equal(headers, readHeaders);

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

	private enum MittoAction : short
	{
		None = 0,
		Auth = 1,
		Produce = 2,
		Connect = 4,
		Disconnect = 8,
		Consume = 16,
		Stream = 32,
		Session = 64,
	}

	private enum MittoActionModifier : byte
	{
		None = 0,
		End = 1,
		Start = 2,
		Ack = 4,
		Nack = 8,
		Error = 16,
	}

	private static class Data
	{
		public const string SmallHeaders = "key1:value1\nkey2:value2";
		public const string SmallBody = "Lorem ipsum dolor sit amet, consectetur.";

		public const string MediumHeaders = "key1:value1\nkey2:value2\nkey3:value3\nkey4:value4\nkey5:value5";
		public const string MediumBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore.";

		public const string LargeHeaders = "key1:value1\nkey2:value2\nkey3:value3\nkey4:value4\nkey5:value5\nkey6:value6\nkey7:value7\nkey8:value8";
		public const string LargeBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
	}
}
