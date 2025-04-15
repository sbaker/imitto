using IMitto.Net;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using System.Diagnostics.CodeAnalysis;

namespace IMitto.Middlware;

public delegate Task MiddlewareFunc(MiddlewareContext context, CancellationToken token);
public delegate Task MiddlewareBuilderFunc(MiddlewareFunc next, MiddlewareContext context, CancellationToken token);

public delegate Task MiddlewareFunc<TContext>(TContext context, CancellationToken token);
public delegate Task MiddlewareBuilderFunc<TContext>(MiddlewareFunc<TContext> next, TContext context, CancellationToken token);







public interface IWrap<T>
{
	T Unwrap();

	static T Unwrap(IWrap<T> context)
	{
		return context.Unwrap();
	}

	static abstract T Unwrap(T context);
}
