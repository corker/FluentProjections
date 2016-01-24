using System;
using FluentProjections.Persistence;

namespace FluentProjections.Strategies
{
    public class MessageHandlingStrategyFactoryContainer<TMessage, TProjection> :
        IContainMessageHandlingStrategyFactory<TMessage, TProjection>
        where TProjection : class, new()
    {
        private Func<IMessageHandlingStrategy<TMessage>> _factory = () => new EmptyStrategy();

        public void SetFactory(Func<IMessageHandlingStrategy<TMessage>> factory)
        {
            _factory = factory;
        }

        public IMessageHandlingStrategy<TMessage> Create()
        {
            return _factory();
        }

        private class EmptyStrategy : IMessageHandlingStrategy<TMessage>
        {
            public void Handle(TMessage message, IProvideProjections store)
            {
            }
        }
    }
}