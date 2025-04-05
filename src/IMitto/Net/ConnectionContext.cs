﻿using IMitto.Net.Server;

namespace IMitto.Net;

public class ConnectionContext : Disposable
{
	private Task? _backgroudTask;

	public ConnectionContext(IMittoEvents eventAggregator, MittoSocket socket, CancellationToken token)
	{
		EventAggregator = eventAggregator;
		Socket = socket;

		token.Register(Dispose);

		eventAggregator.Publish(ServerEventConstants.ConnectionReceivedEvent, this);
	}

	public string ConnectionId { get; } = Guid.NewGuid().ToString();

	public IMittoEvents EventAggregator { get; }
	
	public MittoSocket Socket { get; }

	public Task BackgroundTask// { get; set; }
	{
		get
		{
			return _backgroudTask ?? Task.CompletedTask;
		}
		set
		{
			_backgroudTask = value;
		}
	}

	protected override void DisposeCore()
	{
		Socket.Dispose();

		if (!BackgroundTask.IsCompleted)
		{
			BackgroundTask?.Wait();
		}
	}
}
