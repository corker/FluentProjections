using System;

namespace FluentProjections.EventHandlingStrategies
{
    public class EventHandlingStrategyProvider<TEvent, TProjection> : IEventHandlingStrategyProvider
        where TProjection : class, new()
    {
        private Func<IFluentEventHandlingStrategy<TEvent>> _factory;

        void IEventHandlingStrategyProvider.RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            registerer.Register(Create());
        }

        public void SetFactory(Func<IFluentEventHandlingStrategy<TEvent>> factory)
        {
            _factory = factory;
        }

        public IFluentEventHandlingStrategy<TEvent> Create()
        {
            return _factory();
        }
    }
}