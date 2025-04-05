﻿namespace IMitto.Tests;

    public class EventTests
    {
        [Fact]
        public void EventSubscriptionTest()
        {
            using (var subscription = Subscribe.ToLocalEvent<string>("eventId", s => Assert.True(s.Data == "Event raised.")))
            {
                subscription.Publish("Event raised.");

                subscription.Unsubscribe();
            }
        }
    }
