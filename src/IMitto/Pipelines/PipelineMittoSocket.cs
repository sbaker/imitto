﻿using IMitto.Net.Settings;
using IMitto.Net;
using System.Net.Sockets;

namespace IMitto.Pipelines;

public class PipelineMittoSocket : MittoSocket
{
	private readonly NetworkStream _stream;
	private readonly MittoDuplexPipe _pipeline;

	public PipelineMittoSocket(TcpClient tcpClient, MittoBaseOptions options) : base(tcpClient, options)
	{
		_stream = Add(tcpClient.GetStream(), s => s.Close());
		_pipeline = new MittoDuplexPipe(_stream, options.Pipeline);
	}
}