using System;

namespace FluentProjections.EventHandlingStrategies
{
    public class EventHandlingStrategyFactoryContainer<TEvent, TProjection> : IContainEventHandlingStrategyFactory<TEvent, TProjection>
        where TProjection : class, new()
    {
        private Func<IEventHandlingStrategy<TEvent>> _factory;

        public IEventHandlingStrategy<TEvent> Create()
        {
            return _factory();
        }

        public void SetFactory(Func<IEventHandlingStrategy<TEvent>> factory)
        {
            _factory = factory;
        }
    }
}