using IMitto.Net;
using IMitto.Pipelines;

namespace IMitto.Protocols
{
	public static class MittoProtocol
	{
		public static IProtocolTransport DefaultProtocolTransport { get; } = new V1.MittoProtocolTransport();

		public static IProtocolTransport Create(MittoProtocolVersion version) => version switch
		{
			MittoProtocolVersion.V1 => DefaultProtocolTransport,
			_ => throw new NotSupportedException($"Protocol version {version} is not supported.")
		};

		public static Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default)
		{
			var transport = Create(package.Command.Version);
			return transport.SendAsync(context, package, token);
		}

		public static Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, MittoProtocolVersion version = MittoProtocolVersion.V1, CancellationToken token = default)
		{
			var transport = Create(version);
			return transport.ReadPackageAsync(reader, token);
		}

		public static Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token = default)
		{
			var transport = Create(package.Command.Version);
			return transport.WritePackageAsync(writer, package, token);
		}

		public static Task<IMittoPackage> ReceiveAsync(ConnectionContext context, MittoProtocolVersion version = MittoProtocolVersion.V1, CancellationToken token = default)
		{
			var transport = Create(version);
			return transport.ReceiveAsync(context, token);
		}
	}
}
