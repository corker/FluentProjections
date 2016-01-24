using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.Persistence;
using FluentProjections.Strategies;
using NUnit.Framework;

namespace FluentProjections.Tests
{
    public class MessageHandlerTests
    {
        private class TestMessage
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

        private class TestProvidersFactory : ICreateProjectionProviders
        {
            private readonly IProvideProjections _provider;

            public TestProvidersFactory(IProvideProjections provider)
            {
                _provider = provider;
            }

            public IProvideProjections Create()
            {
                return _provider;
            }
        }

        private class TestProvider : IProvideProjections
        {
            public TestProvider(TestProjection readProjection)
            {
                ReadProjection = readProjection;
            }

            public IEnumerable<FilterValue> ReadFilterValues { get; private set; }
            public TestProjection ReadProjection { get; }
            public TestProjection UpdateProjection { get; private set; }
            public List<TestProjection> InsertProjections { get; private set; }
            public IEnumerable<FilterValue> RemoveFilterValues { get; private set; }

            public IEnumerable<TProjection> Read<TProjection>(IEnumerable<FilterValue> values)
                where TProjection : class
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

            public void Remove<TProjection>(IEnumerable<FilterValue> values) where TProjection : class
            {
                RemoveFilterValues = values;
            }
        }

        private class TestProviderWithUnitOfWork : IProvideProjections, IUnitOfWork
        {
            public bool Committed { get; private set; }

            public IEnumerable<TProjection> Read<TProjection>(IEnumerable<FilterValue> values)
                where TProjection : class
            {
                throw new NotImplementedException();
            }

            public void Update<TProjection>(TProjection projection) where TProjection : class
            {
                throw new NotImplementedException();
            }

            public void Insert<TProjection>(TProjection projection) where TProjection : class
            {
            }

            public void Remove<TProjection>(IEnumerable<FilterValue> values) where TProjection : class
            {
                throw new NotImplementedException();
            }

            public void Commit()
            {
                Committed = true;
            }
        }

        private class TestProviderWithDisposable : IProvideProjections, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public IEnumerable<TProjection> Read<TProjection>(IEnumerable<FilterValue> values)
                where TProjection : class
            {
                throw new NotImplementedException();
            }

            public void Update<TProjection>(TProjection projection) where TProjection : class
            {
                throw new NotImplementedException();
            }

            public void Insert<TProjection>(TProjection projection) where TProjection : class
            {
            }

            public void Remove<TProjection>(IEnumerable<FilterValue> values) where TProjection : class
            {
                throw new NotImplementedException();
            }
        }

        [TestFixture]
        public class When_message_add_new_projection
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => x
                        .AddNew()
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }
            }

            private TestProvider _targetProvider;

            [OneTimeSetUp]
            public void Init()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).Handle(message);
            }

            [Test]
            public void Should_add_new_projection()
            {
                Assert.AreEqual(1, _targetProvider.InsertProjections.Count);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(777, _targetProvider.InsertProjections.Single().ValueInt32);
            }
        }

        [TestFixture]
        public class When_message_remove_projection
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => x
                        .Remove()
                        .WhenEqual(p => p.ValueInt16, 555)
                        .WhenEqual(p => p.ValueInt32, e => e.ValueInt32)
                        .WhenEqual(p => p.ValueInt64));
                }
            }

            private TestProvider _targetProvider;

            [OneTimeSetUp]
            public void Init()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).Handle(message);
            }

            [Test]
            public void Should_filter_projection_by_correct_property()
            {
                Assert.AreEqual("ValueInt32", _targetProvider.RemoveFilterValues.Skip(1).First().Property.Name);
            }

            [Test]
            public void Should_filter_projection_by_correct_property_conventionaly_mapped()
            {
                Assert.AreEqual("ValueInt64", _targetProvider.RemoveFilterValues.Skip(2).First().Property.Name);
            }

            [Test]
            public void Should_filter_projection_by_correct_property_mapped_to_constant()
            {
                Assert.AreEqual("ValueInt16", _targetProvider.RemoveFilterValues.First().Property.Name);
            }

            [Test]
            public void Should_filter_projection_with_correct_value()
            {
                Assert.AreEqual(777, _targetProvider.RemoveFilterValues.Skip(1).First().Value);
            }

            [Test]
            public void Should_filter_projection_with_correct_value_conventionaly_mapped()
            {
                Assert.AreEqual(888, _targetProvider.RemoveFilterValues.Skip(2).First().Value);
            }

            [Test]
            public void Should_filter_projection_with_correct_value_mapped_to_constant()
            {
                Assert.AreEqual(555, _targetProvider.RemoveFilterValues.First().Value);
            }
        }

        [TestFixture]
        public class When_message_save_existing_projection
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => x
                        .Save()
                        .WithKey(p => p.ValueInt16, 555)
                        .WithKey(p => p.ValueInt32, e => e.ValueInt32)
                        .WithKey(p => p.ValueInt64)
                        .Map(p => p.ValueInt64, e => e.ValueInt64));
                }
            }

            private TestProvider _targetProvider;
            private TestProjection _targetProjection;

            [OneTimeSetUp]
            public void Init()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProjection = new TestProjection();
                _targetProvider = new TestProvider(_targetProjection);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).Handle(message);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.AreEqual("ValueInt64", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.AreEqual("ValueInt16", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.AreEqual(888, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.AreEqual(555, value.Value);
            }

            [Test]
            public void Should_keep_key_the_same()
            {
                Assert.AreEqual(0, _targetProvider.UpdateProjection.ValueInt32);
            }

            [Test]
            public void Should_read_from_store()
            {
                Assert.AreSame(_targetProjection, _targetProvider.ReadProjection);
            }

            [Test]
            public void Should_update_with_new_values()
            {
                Assert.AreEqual(888, _targetProvider.UpdateProjection.ValueInt64);
            }

            [Test]
            public void Should_update_with_the_same_projection()
            {
                Assert.AreSame(_targetProjection, _targetProvider.UpdateProjection);
            }
        }

        [TestFixture]
        public class When_message_save_new_projection
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => x
                        .Save()
                        .WithKey(p => p.ValueInt16, 555)
                        .WithKey(p => p.ValueInt32, e => e.ValueInt32)
                        .WithKey(p => p.ValueInt64)
                        .Map(p => p.ValueInt64, e => e.ValueInt64));
                }
            }

            private TestProvider _targetProvider;

            [OneTimeSetUp]
            public void Init()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).Handle(message);
            }

            [Test]
            public void Should_add_new_projection()
            {
                Assert.AreEqual(1, _targetProvider.InsertProjections.Count);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info_conventionally_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.AreEqual("ValueInt64", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.AreEqual("ValueInt16", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value_conventionally_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.AreEqual(888, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.AreEqual(555, value.Value);
            }

            [Test]
            public void Should_map_keys()
            {
                Assert.AreEqual(555, _targetProvider.InsertProjections.Single().ValueInt16);
                Assert.AreEqual(777, _targetProvider.InsertProjections.Single().ValueInt32);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(888, _targetProvider.InsertProjections.Single().ValueInt64);
            }

            [Test]
            public void Should_read_from_store()
            {
                Assert.IsNull(_targetProvider.ReadProjection);
            }
        }

        [TestFixture]
        public class When_message_translated
        {
            private class TestTranslatedMessage
            {
                public int TranslatedValue { get; set; }
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => x
                        .Translate(e => new[]
                        {
                            new TestTranslatedMessage
                            {
                                TranslatedValue = e.ValueInt32
                            },
                            new TestTranslatedMessage
                            {
                                TranslatedValue = e.ValueInt32 + 1
                            }
                        })
                        .AddNew()
                        .Map(p => p.ValueInt32, e => e.TranslatedValue));
                }
            }

            private TestProvider _targetProvider;

            [OneTimeSetUp]
            public void Init()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).Handle(message);
            }

            [Test]
            public void Should_map_values()
            {
                Assert.AreEqual(777, _targetProvider.InsertProjections.First().ValueInt32);
                Assert.AreEqual(778, _targetProvider.InsertProjections.Last().ValueInt32);
            }

            [Test]
            public void Should_translate_message_to_a_list_of_events()
            {
                Assert.AreEqual(2, _targetProvider.InsertProjections.Count);
            }
        }

        [TestFixture]
        public class When_message_update_existing_projection
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => x
                        .Update()
                        .WhenEqual(p => p.ValueInt16, 555)
                        .WhenEqual(p => p.ValueInt32, e => e.ValueInt32)
                        .WhenEqual(p => p.ValueInt64)
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }
            }

            private TestProvider _targetProvider;
            private TestProjection _targetProjection;

            [OneTimeSetUp]
            public void Init()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProjection = new TestProjection();
                _targetProvider = new TestProvider(_targetProjection);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).Handle(message);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.AreEqual("ValueInt32", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.AreEqual("ValueInt64", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_info_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.AreEqual("ValueInt16", value.Property.Name);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.AreEqual(777, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.AreEqual(888, value.Value);
            }

            [Test]
            public void Should_filter_read_result_with_message_property_value_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.AreEqual(555, value.Value);
            }

            [Test]
            public void Should_read_from_store()
            {
                Assert.AreSame(_targetProjection, _targetProvider.ReadProjection);
            }

            [Test]
            public void Should_update_with_new_values()
            {
                Assert.AreEqual(777, _targetProvider.UpdateProjection.ValueInt32);
            }

            [Test]
            public void Should_update_with_the_same_projection()
            {
                Assert.AreSame(_targetProjection, _targetProvider.UpdateProjection);
            }
        }

        [TestFixture]
        public class When_provider_implements
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => { });
                }
            }

            [Test]
            public void a_disposable_should_dispose()
            {
                var provider = new TestProviderWithDisposable();
                var factory = new TestProvidersFactory(provider);
                new TestHandler(factory).Handle(new TestMessage());
                Assert.IsTrue(provider.Disposed);
            }

            [Test]
            public void a_unit_of_work_should_commit()
            {
                var provider = new TestProviderWithUnitOfWork();
                var factory = new TestProvidersFactory(provider);
                new TestHandler(factory).Handle(new TestMessage());
                Assert.IsTrue(provider.Committed);
            }
        }

        [TestFixture]
        public class When_handler_is_already_configured_should_reuse_the_strategy
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestStrategy Strategy { get; } = new TestStrategy();

                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public void Handle(TestMessage message)
                {
                    Handle(message, x => { x.SetFactory(() => Strategy); });
                }
            }

            private class TestStrategy : IMessageHandlingStrategy<TestMessage>
            {
                public int Counter { get; private set; }

                public void Handle(TestMessage message, IProvideProjections store)
                {
                    Counter++;
                }
            }

            [Test]
            public void a_disposable_should_dispose()
            {
                var provider = new TestProviderWithDisposable();
                var factory = new TestProvidersFactory(provider);
                var handler = new TestHandler(factory);

                handler.Handle(new TestMessage());
                handler.Handle(new TestMessage());

                Assert.AreEqual(2, handler.Strategy.Counter);
            }
        }
    }
}