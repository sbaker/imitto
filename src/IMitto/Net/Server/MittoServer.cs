using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Hosting;
using IMitto.Middlware;
using IMitto.Protocols.Models;
using System.Collections.Concurrent;
using System.Threading;

namespace IMitto.Net.Server;

public sealed class MittoServer : MittoHost<MittoServerOptions>, IMittoServer
{
	private Task? _eventManagerTask = null;
	private readonly ILogger<MittoServer> _logger;
	private readonly IMittoRequestHandler _requestHandler;
	private readonly IServerEventManager _eventManager;
	private readonly ITopicManager _topicManager;
	private readonly IBrokerStorage _storage;
	private readonly ConcurrentDictionary<string, IBrokerSession> _sessions;
	private readonly ConcurrentDictionary<string, SemaphoreSlim> _messageLocks;

	public MittoServer(
		IOptions<MittoServerOptions> options,
		ILogger<MittoServer> logger,
		IMittoRequestHandler requestHandler,
		IServerEventManager serverEvents,
		ITopicManager topicManager,
		IBrokerStorage storage,
		ConcurrentDictionary<string, IBrokerSession> sessions) : base(logger, options)
	{
		_logger = logger;
		_requestHandler = requestHandler;
		_eventManager = serverEvents;
		_topicManager = topicManager;
		_storage = storage;
		_sessions = sessions;
		_messageLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
	}

	private Task EventManagerTask => _eventManagerTask ?? Task.CompletedTask;

	public string Name => Options.Name;

	private IMittoServerConnection? Connection { get; set; }

	private static Task<MittoPipelineSocket> AcceptSocket(IMittoServerConnection connection, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return connection.AcceptAsync(token);
	}

	protected override Task RunInternalAsync(CancellationToken token = default)
	{
		_logger.LogTrace("Run Server: start");

		_eventManagerTask = _eventManager.RunAsync(token);

		var middleware = CreateMittoListenerPipeline();

		return Task.Run(async () => {
			var retryCount = 0;

			_logger.LogTrace("Initializing connection: start");
			Connection = MittoConnection.CreateServer(Options);
			_logger.LogTrace("Initializing connection: end");

			while (!token.IsCancellationRequested && Options.MaxConnectionRetries > retryCount++)
			{
				try
				{
					while (!token.IsCancellationRequested)
					{
						_logger.LogTrace("Accepting Connections: start");

						if (!Connection.IsConnected())
						{
							await Connection.ConnectAsync(TokenSource.Token).Await();
						}

						var socket = await AcceptSocket(Connection, TokenSource.Token).Await();

						_logger.LogTrace("Accepting Connections: end;");

						var connectionContext = new ConnectionContext(_eventManager, socket);

						await _eventManager.PublishServerEventAsync(
							ServerEventConstants.ConnectionReceivedEvent,
							connectionContext,
							connectionContext.ConnectionId,
							token).Await();

						_logger.LogTrace("Initializing client/server workflow: start {connectionId}", connectionContext.ConnectionId);

						StartBackgroundTask(ct => middleware.HandleAsync(connectionContext, ct), token);
					}
				}
				catch (TaskCanceledException tce)
				{
					_logger.LogWarning(tce, "Task canceled");
					break;
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Server failed to accept new connection: unknown error.");
				}
			}

			_logger.LogTrace("Run Server: end");

			Connection?.Dispose();

		}, token);
	}

	private IMiddlewareHandler<ConnectionContext> CreateMittoListenerPipeline()
	{
		var innerHandler = CreateRequestPipeline();
		return new MiddlewareBuilder<ConnectionContext>()
			.Add(new MittoServerConnectionContextMiddlewareHandler(Options, _logger, innerHandler))
			.Build();
	}

	private IMiddlewareHandler<MittoConnectionContext> CreateRequestPipeline()
	{
		return new MiddlewareBuilder<MittoConnectionContext>()
			.Add((next, context, ct) => {
				_logger.LogTrace("Server Request Listener: start {connectionId}", context.ConnectionId);


				//if (context.Request.Header.Path == MittoPaths.Auth)
				//{
				//	return _requestHandler.HandleAuthenticationAsync(context, ct);
				//}

				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
				return next(context, ct);
			})
			.Add((next, context, ct) => {
				_logger.LogTrace("Server Request Listener: start {connectionId}", context.ConnectionId);
				//if (context.Request.Header.Path == MittoPaths.Topics && context.Request.Header.Action == MittoEventType.Consume)
				//{
				//	return _requestHandler.HandleAuthorizationAsync(context, ct);
				//}
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
				return next(context, ct);
			})
			.Add((next, context, ct) => {
				_logger.LogTrace("Server Request Listener: start {connectionId}", context.ConnectionId);
				//if (context.Request.Header.Path == MittoPaths.Topics && context.Request.Header.Action == MittoEventType.Produce)
				//{
				//	return _requestHandler.HandleEventNotificationAsync(context, ct);
				//}
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
				return next(context, ct);
			})
			.Build();
	}

	public sealed override async Task StopAsync(CancellationToken? token = null)
	{
		using var cts = TokenSource ?? CancellationTokenSource.CreateLinkedTokenSource(token ?? default);

		await Task.WhenAll(
			StopBackgroundTask(EventManagerTask, cts.Token),
			StopBackgroundTask(StartedTask, cts.Token)
		).Await();

		async Task StopBackgroundTask(Task task, CancellationToken token)
		{
			if (!task.IsCompleted)
			{
				try
				{
					cts.CancelAfter(TimeSpan.FromSeconds(5));
					await task.WaitAsync(cts.Token).Await();
				}
				catch (TaskCanceledException tce)
				{
					_logger.LogWarning(tce, "Timeout waiting for task. Stop background task");
				}
			}
		}
	}

	protected override void DisposeCore()
	{
		StopAsync(TokenSource.Token).Wait();
		TokenSource?.Dispose();
		Connection?.Dispose();
	}
}

public interface IMittoBroker
{
	Task StartAsync(CancellationToken token = default);
	Task StopAsync(CancellationToken token = default);
	Task PublishAsync<T>(string topic, T message, BrokerPublishOptions options = default);
	Task<ISubscription> SubscribeAsync<T>(string topic, Func<BrokerMessage<T>, Task> handler, BrokerSubscribeOptions options = default);
}

public class BrokerPublishOptions
{
	public QualityOfService QoS { get; set; } = QualityOfService.AtMostOnce;
	public bool Retain { get; set; }
	public TimeSpan? Expiry { get; set; }
}

public class BrokerSubscribeOptions
{
	public QualityOfService QoS { get; set; } = QualityOfService.AtMostOnce;
	public bool NoLocal { get; set; }
	public string[] TopicFilters { get; set; }
}

public enum QualityOfService
{
	AtMostOnce = 0,    // Fire and forget
	AtLeastOnce = 1,   // Guaranteed delivery
	ExactlyOnce = 2    // Guaranteed delivery, no duplicates
}

public class BrokerMessage<T>
{
	public string Topic { get; init; }
	public T Payload { get; init; }
	public MessageProperties Properties { get; init; }
	public DateTimeOffset Timestamp { get; init; }
	public string MessageId { get; init; }
}

public class MessageProperties
{
	public string ContentType { get; set; }
	public string CorrelationId { get; set; }
	public Dictionary<string, string> Headers { get; set; }
	public TimeSpan? Expiry { get; set; }
}

public interface ITopicManager
{
	Task<bool> RegisterTopicAsync(string topic, TopicOptions options = default);
	Task<bool> UnregisterTopicAsync(string topic);
	Task<IEnumerable<string>> GetMatchingTopicsAsync(string topicFilter);
}

public class TopicOptions
{
	public bool Persistent { get; set; }
	public int MaxMessageSize { get; set; }
	public TimeSpan? MessageRetention { get; set; }
	public int MaxSubscribers { get; set; }
}

public interface IBrokerStorage
{
	Task StoreMessageAsync<T>(string topic, BrokerMessage<T> message);
	Task<IEnumerable<BrokerMessage<T>>> GetMessagesAsync<T>(string topic, int limit = 100);
	Task DeleteMessagesAsync(string topic, DateTimeOffset before);
	Task<bool> HasSubscribersAsync(string topic);
}

public interface IBrokerSession
{
	string SessionId { get; }
	Task ConnectAsync(BrokerConnectionOptions options);
	Task DisconnectAsync();
	Task<ISubscription> SubscribeAsync<T>(string topic, Func<BrokerMessage<T>, Task> handler);
	Task UnsubscribeAsync(string topic);
}

public class BrokerCredentials
{
	public string ClientId { get; init; }
	public string Username { get; init; }
	public string PasswordHash { get; init; }
	public string[] Roles { get; init; }
	public Dictionary<string, string> Claims { get; init; }
	public DateTimeOffset? ExpiresAt { get; init; }
	public bool IsEnabled { get; init; }
}

public class BrokerConnectionOptions
{
	public string ClientId { get; set; }
	public bool CleanSession { get; set; }
	public TimeSpan? KeepAlive { get; set; }
	public BrokerCredentials Credentials { get; set; }
}

public interface IBrokerAuthenticator
{
	Task<BrokerCredentials> AuthenticateAsync(string clientId, string username, string password);
	Task<bool> AuthorizeTopicAccessAsync(string clientId, string topic, TopicAccess access);
}

public class MittoBroker : IMittoBroker
{
	private readonly IServerEventManager _eventManager;
	private readonly ITopicManager _topicManager;
	private readonly IBrokerStorage _storage;
	private readonly IBrokerAuthenticator _authenticator;

	public MittoBroker(IServerEventManager eventManager, ITopicManager topicManager, IBrokerStorage storage, IBrokerAuthenticator authenticator)
	{
		_eventManager = eventManager;
		_topicManager = topicManager;
		_storage = storage;
		_authenticator = authenticator;
	}

	public async Task StartAsync(CancellationToken token = default)
	{
		// Initialize resources, start listening for messages, etc.
	}

	public async Task StopAsync(CancellationToken token = default)
	{
		// Clean up resources, stop listening for messages, etc.
	}

	public async Task PublishAsync<T>(string topic, T message, BrokerPublishOptions options = default)
	{
		// Validate topic
		if (!await _topicManager.RegisterTopicAsync(topic))
			throw new InvalidTopicException(topic);

		// Create broker message
		var brokerMessage = new BrokerMessage<T>
		{
			Topic = topic,
			Payload = message,
			Timestamp = DateTimeOffset.UtcNow,
			MessageId = Guid.NewGuid().ToString()
		};

		// Store if persistent
		if (options.Retain)
			await _storage.StoreMessageAsync(topic, brokerMessage);

		// Publish to subscribers
		var subscribers = await _topicManager.GetMatchingTopicsAsync(topic);
		foreach (var subscriber in subscribers)
		{
			await _eventManager.PublishAsync(EventId.New(subscriber), brokerMessage);
		}
	}

	public async Task<ISubscription> SubscribeAsync<T>(string topic, Func<BrokerMessage<T>, Task> handler, BrokerSubscribeOptions options = null)
	{
		// Implement subscription logic
		throw new NotImplementedException();
	}
}

[Serializable]
internal class InvalidTopicException : Exception
{
	public InvalidTopicException()
	{
	}

	public InvalidTopicException(string? message) : base(message)
	{
	}

	public InvalidTopicException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}

public class QualityOfServiceHandler
{
	private readonly IBrokerStorage _storage;
	private readonly ConcurrentDictionary<string, SemaphoreSlim> _messageLocks;

	public async Task HandleQoS<T>(
		string topic, 
		BrokerMessage<T> message, 
		QualityOfService qos)
	{
		switch (qos)
		{
			case QualityOfService.AtMostOnce:
				await PublishAtMostOnceAsync(topic, message);
				break;
			case QualityOfService.AtLeastOnce:
				await PublishAtLeastOnceAsync(topic, message);
				break;
			case QualityOfService.ExactlyOnce:
				await PublishExactlyOnceAsync(topic, message);
				break;
		}
	}

	private async Task PublishAtMostOnceAsync<T>(string topic, BrokerMessage<T> message)
	{
		await _storage.StoreMessageAsync(topic, message);
	}

	private async Task PublishAtLeastOnceAsync<T>(string topic, BrokerMessage<T> message)
	{
		var messageLock = _messageLocks.GetOrAdd(message.MessageId, _ => new SemaphoreSlim(1));
		await messageLock.WaitAsync();
		try
		{
			await _storage.StoreMessageAsync(topic, message);
			// Publish and wait for acknowledgment
			// Retry if no acknowledgment received
		}
		finally
		{
			messageLock.Release();
		}
	}

	private async Task PublishExactlyOnceAsync<T>(string topic, BrokerMessage<T> message)
	{
		var messageLock = _messageLocks.GetOrAdd(message.MessageId, _ => new SemaphoreSlim(1));
		await messageLock.WaitAsync();
		try
		{
			// Check if message was already processed
			var existing = await _storage.GetMessagesAsync<T>(topic, 1);
			if (!existing.Any(m => m.MessageId == message.MessageId))
			{
				await _storage.StoreMessageAsync(topic, message);
			}
		}
		finally
		{
			messageLock.Release();
		}
	}
}

// public interface IBrokerAuthenticator
// {
// 	Task<BrokerCredentials> AuthenticateAsync(string clientId, string username, string password);
// 	Task<bool> AuthorizeTopicAccessAsync(string clientId, string topic, TopicAccess access);
// }

public enum TopicAccess
{
	Read,
	Write,
	Subscribe
}

// Phase 1: Extend existing event system
public class MittoBrokerEvents : MittoEvents
{
	private readonly IBrokerAuthenticator _authenticator;
	private readonly IBrokerStorage _storage;

	public async Task<BrokerMessage<T>> PublishBrokerMessageAsync<T>(
		string topic, 
		T payload, 
		BrokerPublishOptions options)
	{
		// 1. Create broker message
		var message = new BrokerMessage<T>
		{
			Topic = topic,
			Payload = payload,
			Properties = new MessageProperties(),
			Timestamp = DateTimeOffset.UtcNow
		};

		// 2. Store if persistent
		if (options.Retain)
		{
			await _storage.StoreMessageAsync(topic, message);
		}

		// 3. Publish using existing system
		var eventId = EventId.New(topic);
		await PublishAsync(eventId, message);

		return message;
	}
}

// Phase 2: Add authentication
public interface IBrokerCredentialStore
{
	Task<BrokerCredentials> GetCredentialsAsync(string clientId);
	Task StoreCredentialsAsync(BrokerCredentials credentials);
	Task DeleteCredentialsAsync(string clientId);
}

public class MittoBrokerAuthenticator : IBrokerAuthenticator
{
	private readonly MittoEvents _events;
	private readonly IBrokerCredentialStore _credentialStore;

	public async Task<BrokerCredentials> AuthenticateAsync(
		string clientId, 
		string username, 
		string password)
	{
		var credentials = await _credentialStore.GetCredentialsAsync(clientId);
		if (credentials == null || !credentials.IsEnabled)
			throw new UnauthorizedAccessException("Invalid credentials");
		return credentials;
	}

	public async Task<bool> AuthorizeTopicAccessAsync(string clientId, string topic, TopicAccess access)
	{
		var credentials = await _credentialStore.GetCredentialsAsync(clientId);
		return credentials?.IsEnabled == true && credentials.Roles.Contains("admin");
	}
}

// Phase 3: Add persistence
public interface IStorageProvider
{
	Task StoreAsync<T>(string key, T value);
	Task<T> GetAsync<T>(string key);
	Task DeleteAsync(string key);
}

public class MittoBrokerStorage : IBrokerStorage
{
	private readonly IStorageProvider _storage;

	public MittoBrokerStorage(IStorageProvider storage)
	{
		_storage = storage ?? throw new ArgumentNullException(nameof(storage));
	}

	public async Task StoreMessageAsync<T>(string topic, BrokerMessage<T> message)
	{
		await _storage.StoreAsync(topic, message);
	}

	public async Task<IEnumerable<BrokerMessage<T>>> GetMessagesAsync<T>(string topic, int limit = 100)
	{
		return await _storage.GetAsync<IEnumerable<BrokerMessage<T>>>(topic) ?? Enumerable.Empty<BrokerMessage<T>>();
	}

	public async Task DeleteMessagesAsync(string topic, DateTimeOffset before)
	{
		await _storage.DeleteAsync(topic);
	}

	public async Task<bool> HasSubscribersAsync(string topic)
	{
		return await _storage.GetAsync<bool>($"{topic}/subscribers");
	}
}
