using System;

namespace FluentProjections.Strategies
{
    public interface IContainMessageHandlingStrategyFactory<TMessage, TProjection> : IMessageHandlingStrategyConfiguration<TMessage, TProjection>
    {
        void SetFactory(Func<IMessageHandlingStrategy<TMessage>> factory);
    }
}