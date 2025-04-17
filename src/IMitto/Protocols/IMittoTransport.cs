using IMitto.Net;
using IMitto.Pipelines;

namespace IMitto.Protocols;

public interface IMittoTransport
{
	static readonly MittoProtocolVersion Version = MittoProtocolVersion.V1;

	MittoProtocolVersion ProtocolVersion { get; }

	Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default);

	Task<IMittoPackage> ReceiveAsync(ConnectionContext context, CancellationToken token = default);

	Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default);

	Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token);
}
