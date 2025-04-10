// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IMittoSampleClient2;
using IMitto.Producers;

var builder = Host.CreateApplicationBuilder();

builder.Logging.AddJsonConsole(options => {
	options.IncludeScopes = true;
});

var produceTopic = "testing-topic-1";
var consumeTopic = "testing-topic-2";
var mittoKey = "MittoAuthenticationKey";
var mittoSecretKey = "MittoAuthenticationSecret";

builder.Services.AddIMitto(configure =>
	configure.AddConsumer<TestPackageConsumer, TestPackage>(consumeTopic)
		.Configure(options => {
			options.AuthenticationKey = builder.Configuration.GetValue<string>(mittoKey);
			options.AuthenticationSecret = builder.Configuration.GetValue<string>(mittoSecretKey);
		})
		.AddProducer<TestPackage>(produceTopic));

var host = builder.Build();

var producer = host.Services.GetRequiredService<IMittoProducerProvider<TestPackage>>();

var timer = new Timer(PublishMessage, null, 5000, 12500);

void PublishMessage(object? state)
{
	var testPackage = new TestPackage
	{
		Goods = "Test Package",
	};
	var producerInstance = producer.GetProducerForTopic(produceTopic);
	producerInstance.ProduceAsync(testPackage).ContinueWith(task => {
		Console.WriteLine($"Produced: {task.Result}");
	});
}

await host.StartAsync();

//await Task.Delay(10000).ContinueWith(async _ => {
//	var testPackage = new TestPackage
//	{
//		Goods = "Test Package",
//	};
//	var producerInstance = producer.GetProducerForTopic(topic);
//	var result = await producerInstance.ProduceAsync(testPackage);
//	Console.WriteLine($"Produced: {result}");
//});

await host.WaitForShutdownAsync();
