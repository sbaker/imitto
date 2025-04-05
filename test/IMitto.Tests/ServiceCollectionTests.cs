using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using IMitto.Local;

namespace IMitto.Tests;

    public class ServiceCollectionTests
    {
        [Fact]
        public void AddEventingSingletonTest()
        {
            IServiceCollection services = new ServiceCollection()
                .AddLocalEvents()
                .AddTransient<TestSubscriber>();

            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var dependent1 = scope.ServiceProvider.GetRequiredService<TestSubscriber>();
                var dependent2 = scope.ServiceProvider.GetRequiredService<TestSubscriber>();
                var aggregator = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
                aggregator.Publish(TestSubscriber.EventId, "expected");

                dependent1.Value.Should().Be("expected");
                dependent1.Value.Should().Be(dependent2.Value);
                dependent1.Aggregator.Should().Be(aggregator);
			dependent2.Aggregator.Should().Be(aggregator);

                aggregator.Unsubscribe(dependent1.Subscription).Should().BeTrue();
			aggregator.Unsubscribe(dependent2.Subscription).Should().BeTrue();
			dependent1.Subscription.Invocations.Should().Be(dependent2.Subscription.Invocations);
            }
        }

        private class TestSubscriber
        {
            public TestSubscriber(IEventAggregator aggregator)
            {
                Aggregator = aggregator;
                Subscription = aggregator.Subscribe<string>(EventId, SetValue);
            }

            private void SetValue(EventContext<string> context)
            {
                Value = context.Data;
            }

            public static string EventId { get; } = "test-event-id";

            public IEventAggregator Aggregator { get; }

            public ISubscription Subscription { get; }

            public string? Value { get; set; }
        }
    }
