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
            public IFluentEventHandler<TestEvent> Handler { get; private set; }

            public void Register<TEvent>(IFluentEventHandler<TEvent> eventHandler)
            {
                Handler = (IFluentEventHandler<TestEvent>) eventHandler;
            }
        }

        private class TestStore : IFluentProjectionStore
        {
            public TestStore(TestProjection readProjection)
            {
                ReadProjection = readProjection;
            }

            public FluentProjectionFilterValues FilterValues { get; private set; }
            public TestProjection ReadProjection { get; private set; }
            public TestProjection UpdateProjection { get; private set; }
            public List<TestProjection> InsertProjections { get; private set; }

            public IEnumerable<TProjection> Read<TProjection>(FluentProjectionFilterValues values)
            {
                FilterValues = values;
                return new[] {ReadProjection}.OfType<TProjection>();
            }

            public void Update<TProjection>(TProjection projection)
            {
                UpdateProjection = projection as TestProjection;
            }

            public void Insert<TProjection>(TProjection projection)
            {
                InsertProjections = InsertProjections ?? new List<TestProjection>();
                InsertProjections.Add(projection as TestProjection);
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
            public void Should_add_new_projection()
            {
                Assert.AreEqual(1, _targetStore.InsertProjections.Count);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(777, _targetStore.InsertProjections.Single().ValueInt32);
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

        [TestFixture]
        public class When_event_translated
        {
            private class TestTranslatedEvent
            {
                public int TranslatedValue { get; set; }
            }

            private class TestConfiguration : FluentProjectionConfiguration<TestProjection>
            {
                public TestConfiguration()
                {
                    ForEvent<TestEvent>()
                        .Translate(e => new[]
                        {
                            new TestTranslatedEvent
                            {
                                TranslatedValue = e.ValueInt32
                            },
                            new TestTranslatedEvent
                            {
                                TranslatedValue = e.ValueInt32 + 1
                            }
                        })
                        .AddNew()
                        .Map(p => p.ValueInt32, e => e.TranslatedValue);
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
            public void Should_translate_event_to_a_list_of_events()
            {
                Assert.AreEqual(2, _targetStore.InsertProjections.Count);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(777, _targetStore.InsertProjections.First().ValueInt32);
                Assert.AreEqual(778, _targetStore.InsertProjections.Last().ValueInt32);
            }
        }
    }
}