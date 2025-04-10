// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

var builder = Host.CreateApplicationBuilder();

//builder.Logging.AddJsonConsole(options => {
//	options.IncludeScopes = true;
//});

builder.Logging.AddSimpleConsole(logging => {
	logging.SingleLine = true;
	logging.ColorBehavior = LoggerColorBehavior.Enabled;
	logging.IncludeScopes = true;
	logging.TimestampFormat = "yyyy-MM-ddThh:mm:ss.ffffff";
	logging.UseUtcTimestamp = true;
});

builder.Services.AddIMittoServer(options => {
	options.Name = "imitto-test-server";
});

var app =  builder.Build();

app.Run();