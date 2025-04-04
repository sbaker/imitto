namespace Transmitto.Tests;

    public class EventAggregatorTests
    {
        [Fact]
        public async Task PublishAsync_ShouldCallPublishMethod()
        {
            // Arrange
            var eventId = EventId.New("TestEvent");
            var data = "Test Data";
            var eventAggregator = new EventAggregator();
            var subscription = eventAggregator.Subscribe(eventId, context =>
            {
                Assert.Equal(data, context.GetData());
            });

            // Act
            await eventAggregator.PublishAsync(eventId, data);

            // Assert
            Assert.Equal(1, subscription.Invocations);
        }
    }
