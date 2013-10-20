using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace FluentProjections.Tests
{
    public class FluentProjectionConfigurationTests
    {
        private class TestEvent
        {
            public int ValueInt32 { get; set; }
        }

        private class TestProjection
        {
            public int ValueInt32 { get; set; }
        }

        private class TestRegisterer : IFluentEventHandlerRegisterer
        {
            public IFluentEventHandler<TestEvent, TestProjection> Handler { get; private set; }

            public void Register<TEvent, TProjection>(IFluentEventHandler<TEvent, TProjection> eventHandler)
            {
                Handler = (IFluentEventHandler<TestEvent, TestProjection>) eventHandler;
            }
        }

        private class TestStore : IFluentProjectionStore<TestProjection>
        {
            public TestStore(TestProjection readProjection)
            {
                ReadProjection = readProjection;
            }

            public FluentProjectionFilterValues FilterValues { get; private set; }
            public TestProjection ReadProjection { get; private set; }
            public TestProjection UpdateProjection { get; private set; }
            public TestProjection InsertProjection { get; private set; }

            public IEnumerable<TestProjection> Read(FluentProjectionFilterValues values)
            {
                FilterValues = values;
                return new[] {ReadProjection};
            }

            public void Update(TestProjection projection)
            {
                UpdateProjection = projection;
            }

            public void Insert(TestProjection projection)
            {
                InsertProjection = projection;
            }
        }

        [TestFixture]
        public class When_event_add_new_projection
        {
            private class TestConfiguration : FluentProjectionConfiguration<TestProjection>
            {
                public TestConfiguration()
                {
                    ForEvent<TestEvent>()
                        .AddNew()
                        .Map(p => p.ValueInt32, e => e.ValueInt32);
                }
            }

            private TestStore _targetStore;
            private TestRegisterer _targetRegisterer;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetRegisterer = new TestRegisterer();

                new TestConfiguration().RegisterBy(_targetRegisterer);

                _targetStore = new TestStore(null);

                var @event = new TestEvent
                {
                    ValueInt32 = 777
                };

                _targetRegisterer.Handler.Handle(@event, _targetStore);
            }

            [Test]
            public void Should_add_new_projection_with_mapped_values()
            {
                Assert.AreEqual(777, _targetStore.InsertProjection.ValueInt32);
            }
        }

        [TestFixture]
        public class When_event_update_existing_projection
        {
            private class TestConfiguration : FluentProjectionConfiguration<TestProjection>
            {
                public TestConfiguration()
                {
                    ForEvent<TestEvent>()
                        .Update()
                        .FilterBy(p => p.ValueInt32, e => e.ValueInt32)
                        .Map(p => p.ValueInt32, e => e.ValueInt32);
                }
            }

            private TestStore _targetStore;
            private TestRegisterer _targetRegisterer;
            private TestProjection _targetProjection;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetRegisterer = new TestRegisterer();
                _targetProjection = new TestProjection();

                new TestConfiguration().RegisterBy(_targetRegisterer);

                _targetStore = new TestStore(_targetProjection);

                var @event = new TestEvent
                {
                    ValueInt32 = 777
                };

                _targetRegisterer.Handler.Handle(@event, _targetStore);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info()
            {
                FluentProjectionFilterValue value = _targetStore.FilterValues.Single();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value()
            {
                FluentProjectionFilterValue value = _targetStore.FilterValues.Single();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_read_from_store()
            {
                Assert.AreSame(_targetProjection, _targetStore.ReadProjection);
            }

            [Test]
            public void Should_update_with_new_values()
            {
                Assert.AreEqual(777, _targetStore.UpdateProjection.ValueInt32);
            }

            [Test]
            public void Should_update_with_the_same_projection()
            {
                Assert.AreSame(_targetProjection, _targetStore.UpdateProjection);
            }
        }
    }
}