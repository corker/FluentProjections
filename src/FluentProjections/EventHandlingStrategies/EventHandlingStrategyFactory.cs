using System;

namespace FluentProjections.EventHandlingStrategies
{
    public class EventHandlingStrategyFactory<TEvent, TProjection> :
        IEventHandlingStrategyFactory,
        IEventHandlingStrategyFactory<TEvent>
        where TProjection : class, new()
    {
        private Func<IEventHandlingStrategy<TEvent>> _factory;

        IEventHandlingStrategy IEventHandlingStrategyFactory.Create()
        {
            return Create();
        }

        public IEventHandlingStrategy<TEvent> Create()
        {
            return _factory();
        }

        public void SetFactoryMethod(Func<IEventHandlingStrategy<TEvent>> factory)
        {
            _factory = factory;
        }
    }
}