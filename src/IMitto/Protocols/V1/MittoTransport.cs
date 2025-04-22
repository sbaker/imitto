using IMitto.Net;
using IMitto.Pipelines;
using System.Buffers;

namespace IMitto.Protocols.V1;

internal sealed class MittoTransport : MittoTransportBase
{
	public override MittoProtocolVersion ProtocolVersion => MittoProtocolVersion.V1;

	public override async Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default)
	{
		var command = await ReadCommandAsync(reader, CommandFactory, token);
		var headers = await ReadHeadersAsync(reader, HeadersFactory, HeaderFactory, token);
		var content = await ReadContentAsync(reader, ContentFactory, token);

		return CreatePackage(command, headers, content);

		MittoCommand CommandFactory(MittoProtocolVersion version, MittoAction action, MittoModifier modifier)
		{
			if (version != ProtocolVersion)
			{
				// Not sure what to do here...for now there is only one version.
				// But consider this scenario when a server has been running 
				// and client updates to latest ahead of server. Right now, though they are
				// paired together in the same bundle and can choose client or server.
				// I think the server needs to be as lenient and backward compatable as possible
				// for the time being. Maybe throw or warn later on if the the server can't parse.
			}

			return new(action, modifier);
		}

		static MittoHeaders HeadersFactory() => [];

		static MittoHeader HeaderFactory(string key, ReadOnlySequence<byte> content)
			=> new(key, content);

		static MittoContent ContentFactory(ReadOnlySequence<byte> content)
			=> new(content);
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

	protected override IMittoPackage CreatePackage(IMittoCommand command, IMittoHeaders headers, IMittoContent content)
	{
		return new MittoPackage(command, headers, content);
	}
}
