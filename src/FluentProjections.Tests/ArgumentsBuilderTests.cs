using System;
using FluentProjections.EventHandlingStrategies;
using NUnit.Framework;

namespace FluentProjections.Tests
{
    public class ArgumentsBuilderTests
    {
        private class TestEvent
        {
            public long EventProperty { get; set; }
            public int MappedByName { get; set; }
        }

        private class TestProjection
        {
            public long ProjectionProperty { get; set; }
            public int MappedByName { get; set; }
            public long NoEventProperty { get; set; }
        }

        [TestFixture]
        public class When_event_mapped_to_projection
        {
            [Test]
            public void Should_add_to_projection()
            {
                // Arrange
                var @event = new TestEvent {EventProperty = 5};
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Add(p => p.ProjectionProperty, e => e.EventProperty);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(10, projection.ProjectionProperty);
            }

            [Test]
            public void Should_add_using_only_projection_property_name()
            {
                // Arrange
                var @event = new TestEvent {MappedByName = 10};
                var projection = new TestProjection {MappedByName = 10};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Add(p => p.MappedByName);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(20, projection.MappedByName);
            }

            [Test]
            public void Should_decrement_projection()
            {
                // Arrange
                var @event = new TestEvent();
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Decrement(p => p.ProjectionProperty);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(4, projection.ProjectionProperty);
            }

            [Test]
            public void Should_do_lambda_expression()
            {
                // Arrange
                var @event = new TestEvent {EventProperty = 5};
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Do((e, p) => p.ProjectionProperty = p.ProjectionProperty*e.EventProperty);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(25, projection.ProjectionProperty);
            }

            [Test]
            public void Should_increment_projection()
            {
                // Arrange
                var @event = new TestEvent();
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Increment(p => p.ProjectionProperty);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(6, projection.ProjectionProperty);
            }

            [Test]
            public void Should_map_to_projection()
            {
                // Arrange
                var @event = new TestEvent {EventProperty = 777};
                var projection = new TestProjection();
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Map(p => p.ProjectionProperty, e => e.EventProperty);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(@event.EventProperty, projection.ProjectionProperty);
            }

            [Test]
            public void Should_set_to_projection()
            {
                // Arrange
                var @event = new TestEvent();
                var projection = new TestProjection();
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Set(p => p.ProjectionProperty, 555);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(555, projection.ProjectionProperty);
            }

            [Test]
            public void Should_map_using_only_projection_property_name()
            {
                // Arrange
                var @event = new TestEvent {MappedByName = 555};
                var projection = new TestProjection();
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Map(p => p.MappedByName);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(@event.MappedByName, projection.MappedByName);
            }

            [Test]
            public void Should_substract_from_projection()
            {
                // Arrange
                var @event = new TestEvent {EventProperty = 5};
                var projection = new TestProjection {ProjectionProperty = 15};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Substract(p => p.ProjectionProperty, e => e.EventProperty);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(10, projection.ProjectionProperty);
            }

            [Test]
            public void Should_substract_using_only_projection_property_name()
            {
                // Arrange
                var @event = new TestEvent {MappedByName = 5};
                var projection = new TestProjection {MappedByName = 15};
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();
                builder.Substract(p => p.MappedByName);

                // Act
                builder.Mappers.Map(@event, projection);

                // Assert
                Assert.AreEqual(10, projection.MappedByName);
            }

            [Test]
            public void Should_throw_if_no_event_property_found_for_conventional_add()
            {
                // Arrange
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();

                // Act
                var @delegate = new TestDelegate(() => builder.Add(p => p.NoEventProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }

            [Test]
            public void Should_throw_if_no_event_property_found_for_conventional_mapping()
            {
                // Arrange
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();

                // Act
                var @delegate = new TestDelegate(() => builder.Map(p => p.NoEventProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }

            [Test]
            public void Should_throw_if_no_event_property_found_for_conventional_substract()
            {
                // Arrange
                var builder = new AddNewProjectionStrategyArguments<TestEvent, TestProjection>();

                // Act
                var @delegate = new TestDelegate(() => builder.Substract(p => p.NoEventProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }
        }
    }
}