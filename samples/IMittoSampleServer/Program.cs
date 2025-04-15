// See https://aka.ms/new-console-template for more information

using IMitto.Net;
using IMitto.Middlware;
using Microsoft.Extensions.Logging.Console;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using System.Diagnostics.CodeAnalysis;
using IMitto.Net.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using IMitto.Net.Server;

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

var route = new MittoTopicTemplate("topics/@@/test", MittoEventType.Consume, (req, token) => Task.FromResult<IMittoResponse>(null!));



builder.Services.AddIMittoServer(options => {
	options.Name = "imitto-test-server";
});

var app =  builder.Build();

var b = new MiddlewareBuilder();
b.AddMittoTopicTemplate("imtt:topics/{topic:string}/{action:enum}", (req, ct) => Task.FromResult<IMittoResponse>(null!));

/*
1st 2bytes/16bits
Action
none=0
auth=1
produce=2
connect=4
disconnect=8
consume=16
stream=32
session=64

2nd 1byte/4bits
none=0
end=1
start=2
ack=4
nack=8
error=16

{Action Desctiptor}|  {Action Headers}  | {Package Contents}
 Action | Modifier |  Length | Headers  |  Length | Content
{16bits | 4bits}   | {32bits}| {Length} | {32bits}| {Length}



connect=>512
<=ack/nack/error=4|8|16

session-start=128|2

authn/authz=>2|1
<=ack/nack/error

comsume=>4
<=ack/nack/error

produce=>8
<=ack/nack


stream=>16
<=ack/nack/error



session-end=128|1
<=ack/nack/error


disconnect=1024

 */

app.Run();


public interface ITopicTemplate
{
	[StringSyntax(StringSyntaxAttribute.CompositeFormat)]
	string Pattern { get; }

	MittoEventType Action { get; }

	RequestHandlerDelegate RequestHandler { get; }
}

public delegate Task<IMittoResponse> RequestHandlerDelegate(IMittoRequest request, CancellationToken token);

public class MittoTopicTemplate([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string pattern, MittoEventType action, RequestHandlerDelegate handler) : ITopicTemplate
{
	[StringSyntax(StringSyntaxAttribute.CompositeFormat)]
	public string Pattern { get; } = pattern;
	
	public MittoEventType Action { get; } = action;

	public RequestHandlerDelegate RequestHandler { get; } = handler;
}

public interface IMittoEndpoint
{
	string Name { get; set; }

	Task ExecuteAsync(IMittoRequest request, CancellationToken token);
}

interface ITopicTemplateEndpoint : IMittoEndpoint
{
	static abstract ITopicTemplate GetTopicTemplate();
}

public abstract class MittoEndpoint : IMittoEndpoint
{
	public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public abstract Task ExecuteAsync(IMittoRequest request, CancellationToken token);
}

public class TopicTemplateEndpoint : MittoEndpoint, ITopicTemplateEndpoint
{
	public static ITopicTemplate GetTopicTemplate()
	{
		return new MittoTopicTemplate(
		"",
		MittoEventType.Consume,
		(req, ct) => Task.FromResult<IMittoResponse>(new MittoResponse<MittoMessageBody>()));
	}

	public override Task ExecuteAsync(IMittoRequest request, CancellationToken token)
	{
		return GetTopicTemplate().RequestHandler switch
		{
			RequestHandlerDelegate handler => handler(request, token),
			_ => throw new NotSupportedException($"Unsupported request handler type: {GetTopicTemplate().RequestHandler.GetType().Name}"),
		};
	}
}

public class TopicTemplateMatcher(string pattern, MittoEventType action)
{
	private readonly string _pattern = pattern;
	private readonly MittoEventType _action = action;

	public bool IsMatch(string topic, MittoEventType action)
	{
		if (string.IsNullOrEmpty(topic))
			throw new ArgumentNullException(nameof(topic));

		return string.Equals(_pattern, topic, StringComparison.OrdinalIgnoreCase) && _action == action;
	}
}

public static class MiddlewareExtensions
{
	public static IMiddlewareBuilder AddMittoTopicTemplate(this IMiddlewareBuilder builder, [StringSyntax(StringSyntaxAttribute.Uri)] string pattern, RequestHandlerDelegate handler)
	{
		return builder;//.UseMiddleware<MittoRouteMiddleware>(pattern, handler);
	}

	private sealed class MittoTopicTemplateMiddleware : IMiddlewareHandler<MittoConnectionContext>
	{
		private readonly string _pattern;
		private readonly RequestHandlerDelegate _handler;

		public MittoTopicTemplateMiddleware(string pattern)
		{
		}

		public Task HandleAsync(MittoConnectionContext context, CancellationToken token)
		{
			if (context.Request.Header.Action == MittoEventType.Produce)
			{

			}

			return Task.CompletedTask;
		}
	}
}