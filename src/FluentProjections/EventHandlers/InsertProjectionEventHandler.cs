using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class InsertProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent>
        where TProjection : new()
    {
        private readonly Mappers<TEvent, TProjection> _mappers;

        public InsertProjectionEventHandler(Mappers<TEvent, TProjection> mappers)
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