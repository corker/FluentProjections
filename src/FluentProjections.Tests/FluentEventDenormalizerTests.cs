using System.Collections.Generic;
using System.Linq;
using FluentProjections.AutoMapper;
using NUnit.Framework;
using AutoMapperMapper = AutoMapper.Mapper;

namespace FluentProjections.Tests
{
    public class FluentEventDenormalizerTests
    {
        private class TestEvent
        {
            public short ValueInt16 { get; set; }
            public int ValueInt32 { get; set; }
            public long ValueInt64 { get; set; }
        }

        private class TestProjection
        {
            public short ValueInt16 { get; set; }
            public int ValueInt32 { get; set; }
            public long ValueInt64 { get; set; }
        }

        private class TestStore : IFluentProjectionStore
        {
            public TestStore(TestProjection readProjection)
            {
                ReadProjection = readProjection;
            }

            public IEnumerable<FluentProjectionFilterValue> ReadFilterValues { get; private set; }
            public TestProjection ReadProjection { get; private set; }
            public TestProjection UpdateProjection { get; private set; }
            public List<TestProjection> InsertProjections { get; private set; }
            public IEnumerable<FluentProjectionFilterValue> RemoveFilterValues { get; private set; }

            public IEnumerable<TProjection> Read<TProjection>(IEnumerable<FluentProjectionFilterValue> values) where TProjection : class
            {
                ReadFilterValues = values;
                return new[] {ReadProjection}.OfType<TProjection>();
            }

            public void Update<TProjection>(TProjection projection) where TProjection : class
            {
                UpdateProjection = projection as TestProjection;
            }

            public void Insert<TProjection>(TProjection projection) where TProjection : class
            {
                InsertProjections = InsertProjections ?? new List<TestProjection>();
                InsertProjections.Add(projection as TestProjection);
            }

            public void Remove<TProjection>(IEnumerable<FluentProjectionFilterValue> values) where TProjection : class
            {
                RemoveFilterValues = values;
            }
        }

        [TestFixture]
        public class When_event_add_new_projection
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .AddNew()
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetStore = new TestStore(null);

                var @event = new TestEvent
                {
                    ValueInt32 = 777
                };

                new TestDenormalizer(_targetStore).Handle(@event);
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
        public class When_event_add_new_projection_and_auto_map_properties
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                static TestDenormalizer()
                {
                    AutoMapperMapper.CreateMap<TestEvent, TestProjection>();
                }

                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x.AddNew().AutoMap());
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetStore = new TestStore(null);

                var @event = new TestEvent
                {
                    ValueInt32 = 777
                };

                new TestDenormalizer(_targetStore).Handle(@event);
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
        public class When_event_remove_projection
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Remove()
                        .WhenEqual(p => p.ValueInt16, 555)
                        .WhenEqual(p => p.ValueInt32, e => e.ValueInt32)
                        .WhenEqual(p => p.ValueInt64));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetStore = new TestStore(null);

                var @event = new TestEvent
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                new TestDenormalizer(_targetStore).Handle(@event);
            }

            [Test]
            public void Should_filter_projection_by_correct_property_mapped_to_constant()
            {
                Assert.AreEqual("ValueInt16", _targetStore.RemoveFilterValues.First().Property.Name);
            }

            [Test]
            public void Should_filter_projection_by_correct_property()
            {
                Assert.AreEqual("ValueInt32", _targetStore.RemoveFilterValues.Skip(1).First().Property.Name);
            }

            [Test]
            public void Should_filter_projection_by_correct_property_conventionaly_mapped()
            {
                Assert.AreEqual("ValueInt64", _targetStore.RemoveFilterValues.Skip(2).First().Property.Name);
            }

            [Test]
            public void Should_filter_projection_with_correct_value_mapped_to_constant()
            {
                Assert.AreEqual(555, _targetStore.RemoveFilterValues.First().Value);
            }

            [Test]
            public void Should_filter_projection_with_correct_value()
            {
                Assert.AreEqual(777, _targetStore.RemoveFilterValues.Skip(1).First().Value);
            }

            [Test]
            public void Should_filter_projection_with_correct_value_conventionaly_mapped()
            {
                Assert.AreEqual(888, _targetStore.RemoveFilterValues.Skip(2).First().Value);
            }
        }

        [TestFixture]
        public class When_event_save_existing_projection
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Save()
                        .WithKey(p => p.ValueInt16, 555)
                        .WithKey(p => p.ValueInt32, e => e.ValueInt32)
                        .WithKey(p => p.ValueInt64)
                        .Map(p => p.ValueInt64, e => e.ValueInt64));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;
            private TestProjection _targetProjection;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetProjection = new TestProjection();

                _targetStore = new TestStore(_targetProjection);

                var @event = new TestEvent
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                new TestDenormalizer(_targetStore).Handle(@event);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info_mapped_to_constant()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.First();
                Assert.AreEqual("ValueInt16", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(1).First();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info_conventionaly_mapped()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(2).First();
                Assert.AreEqual("ValueInt64", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value_mapped_to_constant()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.First();
                Assert.AreEqual(555, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(1).First();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value_conventionaly_mapped()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(2).First();
                Assert.AreEqual(888, value.Value);
            }

            [Test]
            public void Should_keep_key_the_same()
            {
                Assert.AreEqual(0, _targetStore.UpdateProjection.ValueInt32);
            }

            [Test]
            public void Should_read_from_store()
            {
                Assert.AreSame(_targetProjection, _targetStore.ReadProjection);
            }

            [Test]
            public void Should_update_with_new_values()
            {
                Assert.AreEqual(888, _targetStore.UpdateProjection.ValueInt64);
            }

            [Test]
            public void Should_update_with_the_same_projection()
            {
                Assert.AreSame(_targetProjection, _targetStore.UpdateProjection);
            }
        }

        [TestFixture]
        public class When_event_save_new_projection
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Save()
                        .WithKey(p => p.ValueInt16, 555)
                        .WithKey(p => p.ValueInt32, e => e.ValueInt32)
                        .WithKey(p => p.ValueInt64)
                        .Map(p => p.ValueInt64, e => e.ValueInt64));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetStore = new TestStore(null);

                var @event = new TestEvent
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                new TestDenormalizer(_targetStore).Handle(@event);
            }

            [Test]
            public void Should_add_new_projection()
            {
                Assert.AreEqual(1, _targetStore.InsertProjections.Count);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info_mapped_to_constant()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.First();
                Assert.AreEqual("ValueInt16", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(1).First();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info_conventionally_mapped()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(2).First();
                Assert.AreEqual("ValueInt64", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value_mapped_to_constant()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.First();
                Assert.AreEqual(555, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(1).First();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value_conventionally_mapped()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(2).First();
                Assert.AreEqual(888, value.Value);
            }

            [Test]
            public void Should_map_keys()
            {
                Assert.AreEqual(555, _targetStore.InsertProjections.Single().ValueInt16);
                Assert.AreEqual(777, _targetStore.InsertProjections.Single().ValueInt32);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(888, _targetStore.InsertProjections.Single().ValueInt64);
            }

            [Test]
            public void Should_read_from_store()
            {
                Assert.IsNull(_targetStore.ReadProjection);
            }
        }

        [TestFixture]
        public class When_event_translated
        {
            private class TestTranslatedEvent
            {
                public int TranslatedValue { get; set; }
            }

            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
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
                        .Map(p => p.ValueInt32, e => e.TranslatedValue));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetStore = new TestStore(null);

                var @event = new TestEvent
                {
                    ValueInt32 = 777
                };

                new TestDenormalizer(_targetStore).Handle(@event);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(777, _targetStore.InsertProjections.First().ValueInt32);
                Assert.AreEqual(778, _targetStore.InsertProjections.Last().ValueInt32);
            }

            [Test]
            public void Should_translate_event_to_a_list_of_events()
            {
                Assert.AreEqual(2, _targetStore.InsertProjections.Count);
            }
        }

        [TestFixture]
        public class When_event_update_existing_projection
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Update()
                        .WhenEqual(p => p.ValueInt16, 555)
                        .WhenEqual(p => p.ValueInt32, e => e.ValueInt32)
                        .WhenEqual(p => p.ValueInt64)
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;
            private TestProjection _targetProjection;

            [TestFixtureSetUp]
            public void Init()
            {
                _targetProjection = new TestProjection();

                _targetStore = new TestStore(_targetProjection);

                var @event = new TestEvent
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                new TestDenormalizer(_targetStore).Handle(@event);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info_mapped_to_constant()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.First();
                Assert.AreEqual("ValueInt16", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(1).First();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_info_conventionaly_mapped()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(2).First();
                Assert.AreEqual("ValueInt64", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value_mapped_to_constant()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.First();
                Assert.AreEqual(555, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(1).First();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_event_property_value_conventionaly_mapped()
            {
                FluentProjectionFilterValue value = _targetStore.ReadFilterValues.Skip(2).First();
                Assert.AreEqual(888, value.Value);
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
        public class When_handle_event_from_denormalizer
        {
            [SetUp]
            public void Init()
            {
                var projection = new TestProjection();
                _targetStore = new TestStore(projection);
                _targetDenormalizer = new TestDenormalizer(_targetStore);
            }

            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly TestStore _store;

                public TestDenormalizer(TestStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Update()
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }

                public void Handle(object @event)
                {
                    Handle(@event, _store);
                }
            }

            private TestStore _targetStore;
            private TestDenormalizer _targetDenormalizer;

            [Test]
            public void Should_handle_configured_event()
            {
                _targetDenormalizer.Handle(new TestEvent());
                Assert.NotNull(_targetStore.UpdateProjection);
            }

            [Test]
            public void Should_skip_unknown_event()
            {
                _targetDenormalizer.Handle(new object());
                Assert.Null(_targetStore.UpdateProjection);
            }
        }
    }
}