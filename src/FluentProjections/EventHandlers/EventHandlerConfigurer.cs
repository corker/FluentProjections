using System;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class EventHandlerConfigurer<TEvent, TProjection> : IEventHandlerConfigurer
        where TProjection : new()
    {
        private readonly ArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private EventHandlerType _handlerType;

        public EventHandlerConfigurer()
        {
            _argumentsBuilder = new ArgumentsBuilder<TEvent, TProjection>();
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            IFluentEventHandler<TEvent, TProjection> handler = Configure();
            registerer.Register(handler);
        }

        private IFluentEventHandler<TEvent, TProjection> Configure()
        {
            switch (_handlerType)
            {
                case EventHandlerType.Insert:
                    return CreateInsertEventHandler();
                case EventHandlerType.Update:
                    return CreateUpdateEventHandler();
                default:
                    throw new NotImplementedException();
            }
        }

        private IFluentEventHandler<TEvent, TProjection> CreateUpdateEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            FluentProjectionFilters<TEvent> filters = _argumentsBuilder.BuildFilters();
            return new UpdateFluentProjectionEventHandler<TEvent, TProjection>(filters, mappers);
        }

        private IFluentEventHandler<TEvent, TProjection> CreateInsertEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            return new InsertFluentProjectionEventHandler<TEvent, TProjection>(mappers);
        }

        public IEventMapperBuilder<TEvent, TProjection> AddNew()
        {
            _handlerType = EventHandlerType.Insert;
            return _argumentsBuilder;
        }

        public ArgumentsBuilder<TEvent, TProjection> Update()
        {
            _handlerType = EventHandlerType.Update;
            return _argumentsBuilder;
        }

        private enum EventHandlerType
        {
            Insert,
            Update
        }
    }
}