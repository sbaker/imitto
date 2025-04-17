using IMitto.Middlware;

namespace IMitto.Tests.Middleware;

public class MiddlewareBuilderTests
{
	[Fact]
	public void Add_Action_ShouldAddMiddlewareAction()
	{
		// Arrange
		var expected = "test";
		bool middlewareExecuted = false;
		var builder = new MiddlewareBuilder<string>();

		MiddlewareBuilderFunc<string> action = (next, context, token) => {
			middlewareExecuted = true;
			Assert.NotNull(context);
			Assert.Same(expected, context);
			return Task.CompletedTask;
		};

		// Act
		builder.Add(action);

		// Assert
		var handler = builder.Build();
		Assert.NotNull(handler);
	}

	[Fact]
	public void Add_Handler_ShouldAddMiddlewareHandler()
	{
		// Arrange
		var builder = new MiddlewareBuilder<string>();
		var mockHandler = new MockMiddlewareHandler<string>("test");

		// Act
		builder.Add(mockHandler);

		// Assert
		var handler = builder.Build();
		Assert.NotNull(handler);
	}

	[Fact]
	public async Task Build_ShouldReturnHandlerThatExecutesMiddleware()
	{
		// Arrange
		var expected = "test";
		bool middlewareExecuted = false;
		var builder = new MiddlewareBuilder<string>();

		builder.Add((next, context, token) => {
			middlewareExecuted = true;
			Assert.NotNull(context);
			Assert.Same(expected, context);
			return Task.CompletedTask;
		});

		var handler = builder.Build();
		var context = "test";

		// Act
		await handler.HandleAsync(context, CancellationToken.None);

		// Assert
		Assert.True(middlewareExecuted);
	}

	[Fact]
	public async Task Build_ShouldReturnHandlerThatExecutesAllMiddleware()
	{
		// Arrange
		var expected = "test";
		bool middlewareExecuted = false;
		var builder = new MiddlewareBuilder<string>();
		var handler = new MockMiddlewareHandler<string>(expected);

		builder.Add(handler);

		builder.Add((next, context, token) => {
			middlewareExecuted = true;
			Assert.NotNull(context);
			Assert.Same(expected, context);
			return Task.CompletedTask;
		});

		var middleware = builder.Build();
		var context = "test";

		// Act
		await middleware.HandleAsync(context, CancellationToken.None);

		// Assert
		Assert.True(middlewareExecuted);
		Assert.True(handler.MiddlewareExecuted);
		Assert.Same(handler.Expected, context);
	}

	private class MockMiddlewareHandler<T> : IMiddlewareHandler<T>
	{
		public MockMiddlewareHandler(T expected)
		{
			Expected = expected;
		}

		public T Expected { get; set; }

		public bool MiddlewareExecuted { get; set; }

		public Task HandleAsync(T context, CancellationToken token)
		{
			MiddlewareExecuted = true;

			Assert.NotNull(context);
			Assert.Equal(Expected, context);
			return Task.CompletedTask;
		}
	}
}
