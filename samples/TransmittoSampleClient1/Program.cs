// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Transmitto.Handlers;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddTransmitto(configure =>
	configure.AddSubscriber<TestMessageHandler, TestMessage>("testing-topic")
	.Configure(options => {
		options.AuthenticationKey = builder.Configuration.GetValue<string>("TestKey");
		options.AuthenticationSecret = builder.Configuration.GetValue<string>("TestSecret");
	}));

var host = builder.Build();

host.Run();

public class TestMessageHandler : ITransmittoMessageHandler<TestMessage>
{
	public Task<MessageHandlerResult> HandleAsync(TestMessage message)
	{
		throw new NotImplementedException();
	}
}

public class TestMessage
{
	public string Message { get; set; }
}