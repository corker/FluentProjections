using System;

namespace FluentProjections.EventHandlingStrategies
{
    public interface IContainEventHandlingStrategyFactory<TEvent, TProjection> : IEventHandlingStrategyConfiguration<TEvent, TProjection>
    {
        void SetFactory(Func<IEventHandlingStrategy<TEvent>> factory);
    }
}