using FluentAssertions;

namespace Transmitto.Tests;

    public class EventIdTests
    {
        [Fact]
        public void ImplicitStringConversionTests()
        {
            var expected = "expected";
            EventId eventId = "expected";
            string actual = ((EventId<string>)eventId).Value;
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ImplicitIntConversionTests()
        {
            var expected = 100;
            EventId eventId = 100;
            int actual = ((EventId<int>)eventId).Value; ;
            actual.Should().Be(expected);
        }

        [Fact]
        public void ImplicitGenericTypeConversionTests()
        {
            var expected = new Test { Testing = true };
            EventId<Test> eventId = expected;
            eventId.Value.Testing = false;
            Test actual = eventId;
            actual.Should().BeEquivalentTo(expected);
            actual.Testing.Should().Be(expected.Testing);
        }

        private class Test
        {
            public bool Testing { get; set; } = true;
        }
    }
