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

var topic = "testing-topic";
var mittoKey = "MittoAuthenticationKey";
var mittoSecretKey = "MittoAuthenticationSecret";

builder.Services.AddIMitto(configure =>
	configure.AddConsumer<TestPackageConsumer, TestPackage>(topic)
		.Configure(options => {
			options.AuthenticationKey = builder.Configuration.GetValue<string>(mittoKey);
			options.AuthenticationSecret = builder.Configuration.GetValue<string>(mittoSecretKey);
		})
		.AddProducer<TestPackage>(topic));

var host = builder.Build();

host.Run();
