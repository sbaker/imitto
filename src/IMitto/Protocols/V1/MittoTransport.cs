using IMitto.Net;
using IMitto.Pipelines;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Text;

namespace IMitto.Protocols.V1;

internal sealed class MittoTransport : MittoTransportBase
{
	public override MittoProtocolVersion ProtocolVersion => MittoProtocolVersion.V1;

	public override async Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default)
	{
		var command = await ReadCommandAsync(reader, (action, modifier) => new MittoCommand(action, modifier), token);
		var headers = await ReadHeadersAsync(reader, () => new MittoHeaders(), (key, content) => new MittoHeader(key, content), token);
		var content = await ReadContentAsync(reader, content => new MittoContent(content), token);
																  
		return CreatePackage(command, headers, content);
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
