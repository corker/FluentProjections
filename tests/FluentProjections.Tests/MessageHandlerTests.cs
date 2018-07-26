using FluentProjections;
using FluentProjections.Persistence;
using FluentProjections.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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

            public Task<IEnumerable<TProjection>> ReadAsync<TProjection>(IEnumerable<FilterValue> values)
                where TProjection : class
            {
                ReadFilterValues = values;
                var result = new[] {ReadProjection}.OfType<TProjection>();
                return Task.FromResult(result);
            }

            public Task UpdateAsync<TProjection>(TProjection projection) where TProjection : class
            {
                UpdateProjection = projection as TestProjection;
                return Task.FromResult(0);
            }

            public Task InsertAsync<TProjection>(TProjection projection) where TProjection : class
            {
                InsertProjections = InsertProjections ?? new List<TestProjection>();
                InsertProjections.Add(projection as TestProjection);
                return Task.FromResult(0);
            }

            public Task RemoveAsync<TProjection>(IEnumerable<FilterValue> values) where TProjection : class
            {
                RemoveFilterValues = values;
                return Task.FromResult(0);
            }
        }

        private class TestProviderWithUnitOfWork : IProvideProjections, IUnitOfWork
        {
            public bool Committed { get; private set; }

            public Task<IEnumerable<TProjection>> ReadAsync<TProjection>(IEnumerable<FilterValue> values)
                where TProjection : class
            {
                throw new NotImplementedException();
            }

            public Task UpdateAsync<TProjection>(TProjection projection) where TProjection : class
            {
                throw new NotImplementedException();
            }

            public Task InsertAsync<TProjection>(TProjection projection) where TProjection : class
            {
                return Task.FromResult(0);
            }

            public Task RemoveAsync<TProjection>(IEnumerable<FilterValue> values) where TProjection : class
            {
                throw new NotImplementedException();
            }

            public Task CommitAsync()
            {
                Committed = true;
                return Task.FromResult(0);
            }
        }

        private class TestProviderWithDisposable : IProvideProjections, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public Task<IEnumerable<TProjection>> ReadAsync<TProjection>(IEnumerable<FilterValue> values)
                where TProjection : class
            {
                throw new NotImplementedException();
            }

            public Task UpdateAsync<TProjection>(TProjection projection) where TProjection : class
            {
                throw new NotImplementedException();
            }

            public Task InsertAsync<TProjection>(TProjection projection) where TProjection : class
            {
                return Task.FromResult(0);
            }

            public Task RemoveAsync<TProjection>(IEnumerable<FilterValue> values) where TProjection : class
            {
                throw new NotImplementedException();
            }
        }

        public class When_message_add_new_projection
        {
            public When_message_add_new_projection()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).HandleAsync(message).Wait();
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await base.HandleAsync(message, x => x
                        .AddNew()
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }
            }

            private readonly TestProvider _targetProvider;

            [Fact]
            public void Should_add_new_projection()
            {
                Assert.Single(_targetProvider.InsertProjections);
            }

            [Fact]
            public void Should_map_values()
            {
                Assert.Equal(777, _targetProvider.InsertProjections.Single().ValueInt32);
            }
        }

        public class When_message_remove_projection
        {
            public When_message_remove_projection()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).HandleAsync(message).Wait();
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => x
                        .Remove()
                        .WhenEqual(p => p.ValueInt16, 555)
                        .WhenEqual(p => p.ValueInt32, e => e.ValueInt32)
                        .WhenEqual(p => p.ValueInt64));
                }
            }

            private readonly TestProvider _targetProvider;

            [Fact]
            public void Should_filter_projection_by_correct_property()
            {
                Assert.Equal("ValueInt32", _targetProvider.RemoveFilterValues.Skip(1).First().Property.Name);
            }

            [Fact]
            public void Should_filter_projection_by_correct_property_conventionaly_mapped()
            {
                Assert.Equal("ValueInt64", _targetProvider.RemoveFilterValues.Skip(2).First().Property.Name);
            }

            [Fact]
            public void Should_filter_projection_by_correct_property_mapped_to_constant()
            {
                Assert.Equal("ValueInt16", _targetProvider.RemoveFilterValues.First().Property.Name);
            }

            [Fact]
            public void Should_filter_projection_with_correct_value()
            {
                Assert.Equal(777, _targetProvider.RemoveFilterValues.Skip(1).First().Value);
            }

            [Fact]
            public void Should_filter_projection_with_correct_value_conventionaly_mapped()
            {
                Assert.Equal(888L, _targetProvider.RemoveFilterValues.Skip(2).First().Value);
            }

            [Fact]
            public void Should_filter_projection_with_correct_value_mapped_to_constant()
            {
                Assert.Equal(555, _targetProvider.RemoveFilterValues.First().Value);
            }
        }

        public class When_message_save_existing_projection
        {
            public When_message_save_existing_projection()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProjection = new TestProjection();
                _targetProvider = new TestProvider(_targetProjection);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).HandleAsync(message).Wait();
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => x
                        .Save()
                        .WithKey(p => p.ValueInt16, 555)
                        .WithKey(p => p.ValueInt32, e => e.ValueInt32)
                        .WithKey(p => p.ValueInt64)
                        .Map(p => p.ValueInt64, e => e.ValueInt64));
                }
            }

            private readonly TestProvider _targetProvider;
            private readonly TestProjection _targetProjection;

            [Fact]
            public void Should_filter_read_result_with_message_property_info()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.Equal("ValueInt32", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.Equal("ValueInt64", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.Equal("ValueInt16", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.Equal(777, value.Value);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.Equal(888L, value.Value);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.Equal(555, value.Value);
            }

            [Fact]
            public void Should_keep_key_the_same()
            {
                Assert.Equal(0, _targetProvider.UpdateProjection.ValueInt32);
            }

            [Fact]
            public void Should_read_from_store()
            {
                Assert.Same(_targetProjection, _targetProvider.ReadProjection);
            }

            [Fact]
            public void Should_update_with_new_values()
            {
                Assert.Equal(888, _targetProvider.UpdateProjection.ValueInt64);
            }

            [Fact]
            public void Should_update_with_the_same_projection()
            {
                Assert.Same(_targetProjection, _targetProvider.UpdateProjection);
            }
        }

        public class When_message_save_new_projection
        {
            public When_message_save_new_projection()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).HandleAsync(message).Wait();
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => x
                        .Save()
                        .WithKey(p => p.ValueInt16, 555)
                        .WithKey(p => p.ValueInt32, e => e.ValueInt32)
                        .WithKey(p => p.ValueInt64)
                        .Map(p => p.ValueInt64, e => e.ValueInt64));
                }
            }

            private readonly TestProvider _targetProvider;

            [Fact]
            public void Should_add_new_projection()
            {
                Assert.Single(_targetProvider.InsertProjections);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.Equal("ValueInt32", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info_conventionally_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.Equal("ValueInt64", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.Equal("ValueInt16", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.Equal(777, value.Value);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value_conventionally_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.Equal(888L, value.Value);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.Equal(555, value.Value);
            }

            [Fact]
            public void Should_map_keys()
            {
                Assert.Equal(555, _targetProvider.InsertProjections.Single().ValueInt16);
                Assert.Equal(777, _targetProvider.InsertProjections.Single().ValueInt32);
            }

            [Fact]
            public void Should_map_values()
            {
                Assert.Equal(888, _targetProvider.InsertProjections.Single().ValueInt64);
            }

            [Fact]
            public void Should_read_from_store()
            {
                Assert.Null(_targetProvider.ReadProjection);
            }
        }

        public class When_message_translated
        {
            public When_message_translated()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777
                };

                _targetProvider = new TestProvider(null);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).HandleAsync(message).Wait();
            }

            private class TestTranslatedMessage
            {
                public int TranslatedValue { get; set; }
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => x
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

            private readonly TestProvider _targetProvider;

            [Fact]
            public void Should_map_values()
            {
                Assert.Equal(777, _targetProvider.InsertProjections.First().ValueInt32);
                Assert.Equal(778, _targetProvider.InsertProjections.Last().ValueInt32);
            }

            [Fact]
            public void Should_translate_message_to_a_list_of_events()
            {
                Assert.Equal(2, _targetProvider.InsertProjections.Count);
            }
        }

        public class When_message_update_existing_projection
        {
            public When_message_update_existing_projection()
            {
                var message = new TestMessage
                {
                    ValueInt32 = 777,
                    ValueInt64 = 888
                };

                _targetProjection = new TestProjection();
                _targetProvider = new TestProvider(_targetProjection);
                var persistenceFactory = new TestProvidersFactory(_targetProvider);
                new TestHandler(persistenceFactory).HandleAsync(message).Wait();
            }

            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => x
                        .Update()
                        .WhenEqual(p => p.ValueInt16, 555)
                        .WhenEqual(p => p.ValueInt32, e => e.ValueInt32)
                        .WhenEqual(p => p.ValueInt64)
                        .Map(p => p.ValueInt32, e => e.ValueInt32));
                }
            }

            private readonly TestProvider _targetProvider;
            private readonly TestProjection _targetProjection;

            [Fact]
            public void Should_filter_read_result_with_message_property_info()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.Equal("ValueInt32", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.Equal("ValueInt64", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_info_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.Equal("ValueInt16", value.Property.Name);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value()
            {
                var value = _targetProvider.ReadFilterValues.Skip(1).First();
                Assert.Equal(777, value.Value);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value_conventionaly_mapped()
            {
                var value = _targetProvider.ReadFilterValues.Skip(2).First();
                Assert.Equal(888L, value.Value);
            }

            [Fact]
            public void Should_filter_read_result_with_message_property_value_mapped_to_constant()
            {
                var value = _targetProvider.ReadFilterValues.First();
                Assert.Equal(555, value.Value);
            }

            [Fact]
            public void Should_read_from_store()
            {
                Assert.Same(_targetProjection, _targetProvider.ReadProjection);
            }

            [Fact]
            public void Should_update_with_new_values()
            {
                Assert.Equal(777, _targetProvider.UpdateProjection.ValueInt32);
            }

            [Fact]
            public void Should_update_with_the_same_projection()
            {
                Assert.Same(_targetProjection, _targetProvider.UpdateProjection);
            }
        }

        public class When_provider_implements
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => { });
                }
            }

            [Fact]
            public async Task a_disposable_should_dispose()
            {
                var provider = new TestProviderWithDisposable();
                var factory = new TestProvidersFactory(provider);
                await new TestHandler(factory).HandleAsync(new TestMessage());
                Assert.True(provider.Disposed);
            }

            [Fact]
            public async Task a_unit_of_work_should_commit()
            {
                var provider = new TestProviderWithUnitOfWork();
                var factory = new TestProvidersFactory(provider);
                await new TestHandler(factory).HandleAsync(new TestMessage());
                Assert.True(provider.Committed);
            }
        }

        public class When_handler_is_already_configured_should_reuse_the_strategy
        {
            private class TestHandler : MessageHandler<TestProjection>
            {
                public TestHandler(ICreateProjectionProviders providersFactory) : base(providersFactory)
                {
                }

                public TestStrategy Strategy { get; } = new TestStrategy();

                public async Task HandleAsync(TestMessage message)
                {
                    await HandleAsync(message, x => { x.SetFactory(() => Strategy); });
                }
            }

            private class TestStrategy : IMessageHandlingStrategy<TestMessage>
            {
                public int Counter { get; private set; }

                public Task HandleAsync(TestMessage message, IProvideProjections store)
                {
                    Counter++;
                    return Task.FromResult(0);
                }
            }

            [Fact]
            public async Task a_disposable_should_dispose()
            {
                var provider = new TestProviderWithDisposable();
                var factory = new TestProvidersFactory(provider);
                var handler = new TestHandler(factory);

                await handler.HandleAsync(new TestMessage());
                await handler.HandleAsync(new TestMessage());

                Assert.Equal(2, handler.Strategy.Counter);
            }
        }
    }
}