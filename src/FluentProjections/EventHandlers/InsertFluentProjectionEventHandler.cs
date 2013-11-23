using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class InsertFluentProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent>
        where TProjection : new()
    {
        private readonly EventMappers<TEvent, TProjection> _mappers;

        public InsertFluentProjectionEventHandler(EventMappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
        {
            var projection = new TProjection();
            _mappers.Map(@event, projection);
            store.Insert(projection);
        }
    }
}