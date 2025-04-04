// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IMitto.Consumers;

var builder = Host.CreateApplicationBuilder();

builder.Logging.AddJsonConsole(options => {
	options.IncludeScopes = true;
});

builder.Services.AddTransmitto(configure =>
	configure.AddConsumer<TestMessageHandler, TestMessage>("testing-topic")
	.Configure(options => {
		options.AuthenticationKey = builder.Configuration.GetValue<string>("TestKey");
		options.AuthenticationSecret = builder.Configuration.GetValue<string>("TestSecret");
	}));

var host = builder.Build();

host.Run();

public class TestMessageHandler : IMittoMessageConsumer<TestMessage>
{
	public Task<MessageConsumptionResult> ConsumeAsync(TestMessage message)
	{
		throw new NotImplementedException();
	}
}

public class TestMessage
{
	public string Message { get; set; }
}