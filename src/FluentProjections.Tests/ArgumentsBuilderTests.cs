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
        }

        [TestFixture]
        public class When_event_mapped_to_projection
        {
            private TestEvent _targetEvent;
            private TestProjection _targetProjection;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetEvent = new TestEvent
                {
                    SimplyMappedEventProperty = 777,
                    MappedByName = "MappedByName"
                };

                _targetProjection = new TestProjection();

                var builder = new ArgumentsBuilder<TestEvent, TestProjection>();
                builder
                    .Map(p => p.SimplyMappedProjectionProperty, e => e.SimplyMappedEventProperty)
                    .Map(p => p.MappedByName);

                builder.BuildMappers().Map(_targetEvent, _targetProjection);
            }

            [Test]
            public void Should_do_simple_mapping()
            {
                Assert.AreEqual(_targetEvent.SimplyMappedEventProperty, _targetProjection.SimplyMappedProjectionProperty);
            }

            [Test]
            public void Should_do_mapping_by_projection_property_name()
            {
                Assert.AreEqual(_targetEvent.MappedByName, _targetProjection.MappedByName);
            }
        }
    }
}