using System;
using System.Collections.Concurrent;
using FluentProjections.Persistence;
using FluentProjections.Strategies;

namespace FluentProjections
{
    /// <summary>
    ///     A base class for your message handlers.
    /// </summary>
    /// <typeparam name="TProjection">A type to project messages on</typeparam>
    public abstract class MessageHandler<TProjection> where TProjection : class, new()
    {
        private readonly Handlers _handlers;
        private readonly ICreateProjectionProviders _providersFactory;

        protected MessageHandler(ICreateProjectionProviders providersFactory)
        {
            _providersFactory = providersFactory;
            _handlers = new Handlers();
        }

        /// <summary>
        ///     Handle message with configured strategy.
        /// </summary>
        /// <param name="message">A message to be handled</param>
        /// <param name="configurer">A an action to configure a message strategy</param>
        protected void Handle<TMessage>(
            TMessage message,
            Action<MessageHandlingStrategyFactoryContainer<TMessage, TProjection>> configurer
            )
        {
            var handler = _handlers.GetOrCreateWith(configurer);
            var projections = _providersFactory.Create();

            try
            {
                handler.Handle(message, projections);
                var uow = projections as IUnitOfWork;
                uow?.Commit();
            }
            finally
            {
                var disposable = projections as IDisposable;
                disposable?.Dispose();
            }
        }

        private class Handlers
        {
            private readonly ConcurrentDictionary<object, object> _handlers;

            public Handlers()
            {
                _handlers = new ConcurrentDictionary<object, object>();
            }

            public IHandleMessages<TMessage> GetOrCreateWith<TMessage>(
                Action<MessageHandlingStrategyFactoryContainer<TMessage, TProjection>> configurer
                )
            {
                object handler;
                if (_handlers.TryGetValue(configurer, out handler))
                {
                    return (IHandleMessages<TMessage>) handler;
                }
                handler = new MessageHandler<TMessage>(configurer);
                if (_handlers.TryAdd(configurer, handler))
                {
                    return (IHandleMessages<TMessage>) handler;
                }
                if (_handlers.TryGetValue(configurer, out handler))
                {
                    return (IHandleMessages<TMessage>) handler;
                }
                throw new InvalidOperationException();
            }

            private class MessageHandler<TMessage> : IHandleMessages<TMessage>
            {
                private readonly Action<MessageHandlingStrategyFactoryContainer<TMessage, TProjection>> _configurer;
                private MessageHandlingStrategyFactoryContainer<TMessage, TProjection> _factoryContainer;
                private IMessageHandlingStrategy<TMessage> _strategy;

                public MessageHandler(Action<MessageHandlingStrategyFactoryContainer<TMessage, TProjection>> configurer)
                {
                    _configurer = configurer;
                }

                public void Handle(TMessage message, IProvideProjections projections)
                {
                    EnsureStrategy();
                    _strategy.Handle(message, projections);
                }

                private void EnsureStrategy()
                {
                    EnsureStrategyFactory();
                    _strategy = _strategy ?? _factoryContainer.Create();
                }

                private void EnsureStrategyFactory()
                {
                    if (_factoryContainer == null)
                    {
                        _factoryContainer = new MessageHandlingStrategyFactoryContainer<TMessage, TProjection>();
                        _configurer(_factoryContainer);
                    }
                }
            }
        }

        private interface IHandleMessages<in TMessage>
        {
            void Handle(TMessage message, IProvideProjections projections);
        }
    }
}