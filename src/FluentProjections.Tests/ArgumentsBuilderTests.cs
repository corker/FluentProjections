using System;
using FluentProjections.EventHandlers.Arguments;
using NUnit.Framework;

namespace FluentProjections.Tests
{
    public class ArgumentsBuilderTests
    {
        private class TestEvent
        {
            public long SimplyMappedEventProperty { get; set; }
            public string MappedByName { get; set; }
        }

        private class TestProjection
        {
            public long SimplyMappedProjectionProperty { get; set; }
            public string MappedByName { get; set; }
            public string NoEventProperty { get; set; }
        }

        [TestFixture]
        public class When_event_mapped_to_projection
        {
            [Test]
            public void Should_do_mapping_by_projection_property_name()
            {
                // Arrange
                var @event = new TestEvent {MappedByName = "MappedByName"};
                var projection = new TestProjection();
                var builder = new ArgumentsBuilder<TestEvent, TestProjection>();
                builder.Map(p => p.MappedByName);

                // Act
                builder.BuildMappers().Map(@event, projection);

                // Assert
                Assert.AreEqual(@event.MappedByName, projection.MappedByName);
            }

            [Test]
            public void Should_do_simple_mapping()
            {
                // Arrange
                var @event = new TestEvent {SimplyMappedEventProperty = 777};
                var projection = new TestProjection();
                var builder = new ArgumentsBuilder<TestEvent, TestProjection>();
                builder.Map(p => p.SimplyMappedProjectionProperty, e => e.SimplyMappedEventProperty);

                // Act
                builder.BuildMappers().Map(@event, projection);

                // Assert
                Assert.AreEqual(@event.SimplyMappedEventProperty, projection.SimplyMappedProjectionProperty);
            }

            [Test]
            public void Should_throw_if_no_event_property_found()
            {
                // Arrange
                var builder = new ArgumentsBuilder<TestEvent, TestProjection>();

                // Act
                var @delegate = new TestDelegate(() => builder.Map(p => p.NoEventProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }
        }
    }
}