using IMitto.Net;
using IMitto.Pipelines;
using System;

namespace IMitto.Protocols;

public static class MittoProtocol
{
	internal static IMittoTransport DefaultProtocolTransport { get; } = new V1.MittoTransport();

	public static IMittoTransport CreateTransport(MittoProtocolVersion version) => version switch
	{
		MittoProtocolVersion.V1 => DefaultProtocolTransport,
		_ => throw new NotSupportedException($"Protocol version {version} is not supported.")
	};

	public static IPackageBuilder CreatePackageBuilder(MittoProtocolVersion version = MittoProtocolVersion.V1)
		=> version switch
		{
			MittoProtocolVersion.V1 => new V1.MittoPackageBuilder(),
			_ => throw new NotSupportedException($"Protocol version {version} is not supported.")
		};

	public static Task SendAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default)
	{
		var version = package.Command.Version;
		var transport = CreateTransport(version);
		return transport.SendAsync(context, package, token);
	}

	public static Task<IMittoPackage> ReadPackageAsync(MittoPipeReader reader, MittoProtocolVersion version = MittoProtocolVersion.V1, CancellationToken token = default)
	{
		var transport = CreateTransport(version);
		return transport.ReadPackageAsync(reader, token);
	}

	public static Task WritePackageAsync(MittoPipeWriter writer, IMittoPackage package, CancellationToken token = default)
	{
		var transport = CreateTransport(package.Command.Version);
		return transport.WritePackageAsync(writer, package, token);
	}

	public static Task<IMittoPackage> ReceiveAsync(ConnectionContext context, MittoProtocolVersion version = MittoProtocolVersion.V1, CancellationToken token = default)
	{
		var transport = CreateTransport(version);
		return transport.ReceiveAsync(context, token);
	}
}