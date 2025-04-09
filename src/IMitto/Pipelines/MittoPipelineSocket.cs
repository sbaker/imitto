using IMitto.Net;
using System.Net.Sockets;
using IMitto.Settings;

namespace IMitto.Pipelines;

public class MittoPipelineSocket : MittoSocket
{
	private readonly NetworkStream _stream;
	private readonly MittoDuplexPipe _pipeline;

	public MittoPipelineSocket(TcpClient tcpClient, MittoOptions options) : base(tcpClient, options)
	{
		_stream = Add(tcpClient.GetStream(), s => s.Close());
		_pipeline = new MittoDuplexPipe(_stream, options.Pipeline);
	}

	public override Task<TMessage?> ReadAsync<TMessage>(CancellationToken token = default) where TMessage : default
	{
		if (Options.EnableSocketPipelines)
		{
			return _pipeline.Reader.ReadValueAsync<TMessage?>(token).AsTask();
		}

		return base.ReadAsync<TMessage>(token);
	}

	public override Task SendAsync<TMessage>(TMessage message, CancellationToken token = default)
	{
		if (Options.EnableSocketPipelines)
		{
			return _pipeline.Writer.WriteAsync(message, token);
		}

		return base.SendAsync(message, token);
	}
}