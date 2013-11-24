using System;

namespace FluentProjections.EventHandlers
{
    public class FluentEventHandlerProvider<TEvent, TProjection> : IFluentEventHandlerProvider
        where TProjection : class, new()
    {
        private Func<IFluentEventHandler<TEvent>> _factory;

        void IFluentEventHandlerProvider.RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            registerer.Register(Create());
        }

        public void SetFactory(Func<IFluentEventHandler<TEvent>> factory)
        {
            _factory = factory;
        }

        public IFluentEventHandler<TEvent> Create()
        {
            return _factory();
        }
    }
}