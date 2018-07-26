using System;
using FluentProjections.Strategies;
using Xunit;

namespace FluentProjections.Tests
{
    public class ArgumentsBuilderTests
    {
        private class TestMessage
        {
            public long MessageProperty { get; set; }
            public int MappedByName { get; set; }
        }

        private class TestProjection
        {
            public long ProjectionProperty { get; set; }
            public int MappedByName { get; set; }
            public long NoMessageProperty { get; set; }
        }

        public class When_message_mapped_to_projection
        {
            [Fact]
            public void Should_add_to_projection()
            {
                // Arrange
                var message = new TestMessage {MessageProperty = 5};
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Add(p => p.ProjectionProperty, e => e.MessageProperty);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(10, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_add_using_only_projection_property_name()
            {
                // Arrange
                var message = new TestMessage {MappedByName = 10};
                var projection = new TestProjection {MappedByName = 10};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Add(p => p.MappedByName);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(20, projection.MappedByName);
            }

            [Fact]
            public void Should_decrement_projection()
            {
                // Arrange
                var message = new TestMessage();
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Decrement(p => p.ProjectionProperty);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(4, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_do_lambda_expression()
            {
                // Arrange
                var message = new TestMessage {MessageProperty = 5};
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Do((e, p) => p.ProjectionProperty = p.ProjectionProperty * e.MessageProperty);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(25, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_increment_projection()
            {
                // Arrange
                var message = new TestMessage();
                var projection = new TestProjection {ProjectionProperty = 5};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Increment(p => p.ProjectionProperty);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(6, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_map_to_projection()
            {
                // Arrange
                var message = new TestMessage {MessageProperty = 777};
                var projection = new TestProjection();
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Map(p => p.ProjectionProperty, e => e.MessageProperty);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(message.MessageProperty, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_map_using_only_projection_property_name()
            {
                // Arrange
                var message = new TestMessage {MappedByName = 555};
                var projection = new TestProjection();
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Map(p => p.MappedByName);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(message.MappedByName, projection.MappedByName);
            }

            [Fact]
            public void Should_set_to_projection()
            {
                // Arrange
                var message = new TestMessage();
                var projection = new TestProjection();
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Set(p => p.ProjectionProperty, 555);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(555, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_substract_from_projection()
            {
                // Arrange
                var message = new TestMessage {MessageProperty = 5};
                var projection = new TestProjection {ProjectionProperty = 15};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Substract(p => p.ProjectionProperty, e => e.MessageProperty);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(10, projection.ProjectionProperty);
            }

            [Fact]
            public void Should_substract_using_only_projection_property_name()
            {
                // Arrange
                var message = new TestMessage {MappedByName = 5};
                var projection = new TestProjection {MappedByName = 15};
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();
                builder.Substract(p => p.MappedByName);

                // Act
                builder.Mappers.Map(message, projection);

                // Assert
                Assert.Equal(10, projection.MappedByName);
            }

            [Fact]
            public void Should_throw_if_no_message_property_found_for_conventional_add()
            {
                // Arrange
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();

                // Act
                var @delegate = new Action(() => builder.Add(p => p.NoMessageProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }

            [Fact]
            public void Should_throw_if_no_message_property_found_for_conventional_mapping()
            {
                // Arrange
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();

                // Act
                var @delegate = new Action(() => builder.Map(p => p.NoMessageProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }

            [Fact]
            public void Should_throw_if_no_message_property_found_for_conventional_substract()
            {
                // Arrange
                var builder = new AddNewProjectionStrategyArguments<TestMessage, TestProjection>();

                // Act
                var @delegate = new Action(() => builder.Substract(p => p.NoMessageProperty));

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(@delegate);
            }
        }
    }
}