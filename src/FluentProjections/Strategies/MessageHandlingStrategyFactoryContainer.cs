using System;
using System.Threading.Tasks;
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
            public Task HandleAsync(TMessage message, IProvideProjections store)
            {
                return Task.FromResult(0);
            }
        }
    }
}