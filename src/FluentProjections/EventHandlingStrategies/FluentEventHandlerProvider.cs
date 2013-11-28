using System;

namespace FluentProjections.EventHandlingStrategies
{
    public class FluentEventHandlerProvider<TEvent, TProjection> : IFluentEventHandlerProvider
        where TProjection : class, new()
    {
        private Func<IFluentEventHandlingStrategy<TEvent>> _factory;

        void IFluentEventHandlerProvider.RegisterBy(IFluentEventHandlerRegisterer registerer)
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