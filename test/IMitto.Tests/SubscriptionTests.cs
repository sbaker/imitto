namespace IMitto.Tests;

    public class SubscriptionTests
    {
        [Fact]
        public void SubscribeToStringEventIdTest()
        {
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            var sub = aggregator.Subscribe<IEnumerable<string>>("my-strings", list => Assert.All(list.Data, s => Assert.Contains(s, data)));

            aggregator.Publish("my-strings", data);

            Assert.True(sub.Unsubscribe());
        }

        [Fact]
        public void SubscribeToThrowsWithInvalidDataStringEventIdTest()
        {
            int[] invalidData = [1, 2];
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            var sub = aggregator.Subscribe<IEnumerable<string>>("my-strings", list => Assert.All(list.Data, s => Assert.Contains(s, data)));

            Assert.Throws<ArgumentException>(() => aggregator.Publish("my-strings", invalidData));

            Assert.True(sub.Unsubscribe());
        }

        [Fact]
        public void SubscribeToIncrementsInvocationWhenRaisedStringEventIdTest()
        {
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            var sub = aggregator.Subscribe<IEnumerable<string>>("my-strings", list => Assert.All(list.Data, s => Assert.Contains(s, data)));

            aggregator.Publish("my-strings", data);

            Assert.Equal(1, sub.Invocations);

            Assert.True(sub.Unsubscribe());
        }

        [Fact]
        public void SubscribeToIntEventIdTest()
        {
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            var sub = aggregator.Subscribe<IEnumerable<string>>(1, list => Assert.All(list.Data, s => Assert.Contains(s, data)));

            aggregator.Publish(1, data);

            Assert.True(sub.Unsubscribe());
        }

        [Fact]
        public void MultipleSubscribeToIntEventIdTest()
        {
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            var sub1 = aggregator.Subscribe<IEnumerable<string>>("asdf", list => throw new Exception());

            var sub2 = aggregator.Subscribe<IEnumerable<string>>(1, list => Assert.All(list.Data, s => Assert.Contains(s, data)));

            aggregator.Publish(1, data);

            Assert.True(sub1.Unsubscribe());
            Assert.True(sub2.Unsubscribe());
        }

        [Fact]
        public void MultipleSubscribeToUnsubscribeStringEventIdTest()
        {
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            var sub1 = aggregator.Subscribe<IEnumerable<string>>("asdf", list =>
            {
                // Should never get here
                throw new Exception();
            });

            var sub2 = aggregator.Subscribe<IEnumerable<string>>(1, list => Assert.All(list.Data, s => Assert.Contains(s, data)));

            Assert.True(sub1.Unsubscribe());

            // Should not throw an Exception here since we unsubscribed above.
            aggregator.Publish("asdf", data);

            aggregator.Publish(1, data);

            Assert.True(sub2.Unsubscribe());
        }

        [Fact]
        public void SubscribeToDisposeUnsubscribesStringAndIntEventIdTest()
        {
            string[] data = ["String1", "String2"];

            var aggregator = new EventAggregator();

            ISubscription sub1;
            ISubscription sub2;

            using (sub1 = aggregator.Subscribe<IEnumerable<string>>("asdf", list => { throw new Exception(); }))
            {
                sub2 = aggregator.Subscribe<IEnumerable<string>>(1, list => Assert.All(list.Data, s => Assert.Contains(s, data)));
            }

            // Should not throw an Exception here since we disposed of the subscription above.
            aggregator.Publish("asdf", data);

            Assert.True(((Subscription)sub1).Released);

            aggregator.Publish(1, data);

            sub2.Dispose();

            Assert.True(((Subscription)sub2).Released);
        }

        [Fact]
        public void SubscribeWithParameterlessHandlerTest()
        {
            string[] data = { "String1", "String2" };

            var aggregator = new EventAggregator();

            var sub = aggregator.Subscribe("my-strings", c => { });

            Assert.True(sub.Unsubscribe());
        }

        [Fact]
        public void SubscribeWithSubscriptionFactoryCreatesSubscriptionTest()
        {
            string[] data = { "String1", "String2" };

            var aggregator = new EventAggregator();

            var sub = aggregator.Subscribe("my-strings",
                (agg, id) => new ActionSubscription<IEnumerable<string>>(
                    id,
                    aggregator,
                    c => Assert.NotEmpty(c.Data)));

            Assert.True(sub.Unsubscribe());
        }
    }
