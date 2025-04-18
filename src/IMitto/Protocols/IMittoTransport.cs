using IMitto.Net;
using IMitto.Pipelines;

namespace IMitto.Protocols;

public interface IMittoTransport : IMittoTransport<IMittoPackage>
{
}

public interface IMittoTransport<TPackage> where TPackage  : IMittoPackage
{
	static readonly MittoProtocolVersion Version = MittoProtocolVersion.V1;

	MittoProtocolVersion ProtocolVersion { get; }

	Task<TPackage> ReadPackageAsync(MittoPipeReader reader, CancellationToken token = default);

	Task WritePackageAsync(MittoPipeWriter writer, TPackage package, CancellationToken token = default);

	Task SendAsync(ConnectionContext context, TPackage package, CancellationToken token = default);

	Task<TPackage> ReceiveAsync(ConnectionContext context, CancellationToken token = default);
}
