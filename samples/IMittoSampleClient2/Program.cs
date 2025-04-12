// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IMittoSampleClient2;
using IMitto.Producers;
using Microsoft.Extensions.Logging.Console;

var builder = Host.CreateApplicationBuilder();

builder.Logging.AddSimpleConsole(options => {
	options.IncludeScopes = false;
	options.ColorBehavior = LoggerColorBehavior.Enabled;
	options.TimestampFormat = "[HH:mm:ss] ";
});

var produceTopic = "testing-topic-1";
var produceTopic2 = "testing-topic-2";
var consumeTopic = "testing-topic-1";
var consumeTopic2 = "testing-topic-2";

var mittoKey = "MittoAuthenticationKey";
var mittoSecretKey = "MittoAuthenticationSecret";

builder.Services.AddIMitto(configure =>
	configure.AddProducer<TestPackage>(produceTopic)
		.AddProducer<SubscriberInfo>(produceTopic2)
		.AddConsumer<TestPackageConsumer, TestPackage>(consumeTopic2)
		.AddConsumer<TestPackageConsumer2, TestPackage>(consumeTopic)
		.AddConsumer<NewSubscriberConsumer, SubscriberInfo>(consumeTopic)
		.Configure(options => {
			//options.EnableSocketPipelines = true;
			options.AuthenticationKey = builder.Configuration.GetValue<string>(mittoKey);
			options.AuthenticationSecret = builder.Configuration.GetValue<string>(mittoSecretKey);
		}));

var host = builder.Build();

var testPackageCounter = 0;
var newSubscriberAgecounter = 0;

var testPackageProducer = host.Services.GetRequiredService<IMittoProducerProvider<TestPackage>>();
var newSubscriberProducer = host.Services.GetRequiredService<IMittoProducerProvider<SubscriberInfo>>();

await host.StartAsync();

var testPackageProducerTimer = new Timer(PublishMessageTestPackage, null, 5000, (int)TimeSpan.FromSeconds(60).TotalMilliseconds);
var newSubscriberProducerTimer = new Timer(PublishMessageNewSubscriber, null, 5000, (int)TimeSpan.FromSeconds(15).TotalMilliseconds);


void PublishMessageNewSubscriber(object? state)
{
	var newSubscriber = new SubscriberInfo
	{
		Age = ++newSubscriberAgecounter,
		Username = "imitto",
		SubscribedAt = DateTime.UtcNow,
	};
	var producerInstance = newSubscriberProducer.GetProducerForTopic(produceTopic2);
	producerInstance.ProduceAsync(newSubscriber).ContinueWith(task => {
		Console.WriteLine($"Produced: {task.Result}");
	});
}

void PublishMessageTestPackage(object? state)
{
	var testPackage = new TestPackage
	{
		Goods = $"Test Package {++testPackageCounter}",
	};
	var producerInstance = testPackageProducer.GetProducerForTopic(produceTopic);
	producerInstance.ProduceAsync(testPackage).ContinueWith(task => {
		Console.WriteLine($"Produced: {task.Result}");
	});
}

await host.WaitForShutdownAsync();
