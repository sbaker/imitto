
using IMitto.Local;
using IMitto.Net.Models;
using IMitto.Net.Requests;

namespace IMitto.Net.Server;

public class ClientConnectionContext : Disposables
{
	private readonly object _subscription;

	public ClientConnectionContext(ConnectionContext connection, Task eventLoopTask, CancellationToken token)
	{
		Connection = connection;
		EventLoopTask = eventLoopTask;
	}

	public string ConnectionId => Connection.ConnectionId;

	public ConnectionContext Connection { get; private set; }

	public Task EventLoopTask { get; private set; }

	public void SubscribeToEvents()
	{
		var topics = Connection.Topics;
		var eventAggregator = Connection.EventAggregator;
		var subscribeEvents = topics?.ConsumeTopics?.Select(t => 
			eventAggregator.Subscribe(t, (e, eId) => new ActionSubscription(eId, e, OnEvent))
		).ToArray();
		Add(subscribeEvents ?? []);
	}

	private async void OnEvent(EventContext context)
	{
		await Connection.Socket.SendAsync(new EventNotificationRequest(new() { Content = (EventNotificationsModel)context.GetData() }), CancellationToken.None);
	}
}
