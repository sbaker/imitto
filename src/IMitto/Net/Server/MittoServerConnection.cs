﻿namespace IMitto.Net.Server;

public abstract class MittoServerConnection : MittoConnection, IMittoServerConnection
{
	protected MittoServerConnection(MittoServerOptions options)
	{
		Options = options;
	}

	protected MittoServerOptions Options { get; set; }

	public abstract Task<MittoPipelineSocket> AcceptAsync(CancellationToken token = default);
}
