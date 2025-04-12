// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IMittoSampleClient1;

var builder = Host.CreateApplicationBuilder();

builder.Logging.AddJsonConsole(options => {
	options.IncludeScopes = true;
});

var consumeTopic = "testing-topic-1";
var produceTopic = "testing-topic-2";
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

host.Run();
