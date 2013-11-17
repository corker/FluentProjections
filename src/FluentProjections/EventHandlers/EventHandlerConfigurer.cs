using System;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class EventHandlerConfigurer<TEvent, TProjection> : IEventHandlerConfigurer
        where TProjection : new()
    {
        private readonly ArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private Func<IFluentEventHandler<TEvent, TProjection>> _createHandler;

        public EventHandlerConfigurer()
        {
            _argumentsBuilder = new ArgumentsBuilder<TEvent, TProjection>();
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            IFluentEventHandler<TEvent, TProjection> handler = _createHandler();
            registerer.Register(handler);
        }

        public IEventMapperBuilder<TEvent, TProjection> AddNew()
        {
            _createHandler = CreateInsertEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent, TProjection> CreateInsertEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            return new InsertFluentProjectionEventHandler<TEvent, TProjection>(mappers);
        }

        public ArgumentsBuilder<TEvent, TProjection> Update()
        {
            _createHandler = CreateUpdateEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent, TProjection> CreateUpdateEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            FluentProjectionFilters<TEvent> filters = _argumentsBuilder.BuildFilters();
            return new UpdateFluentProjectionEventHandler<TEvent, TProjection>(filters, mappers);
        }
    }
}