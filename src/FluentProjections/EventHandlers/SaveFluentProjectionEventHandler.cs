using System.Linq;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class SaveFluentProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent>
        where TProjection : class, new()
    {
        private readonly ProjectionKeys<TEvent, TProjection> _keys;
        private readonly EventMappers<TEvent, TProjection> _mappers;

        public SaveFluentProjectionEventHandler(
            ProjectionKeys<TEvent, TProjection> keys,
            EventMappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
            _keys = keys;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
        {
            FluentProjectionFilterValues filterValues = _keys.GetValues(@event);
            TProjection projection = store.Read<TProjection>(filterValues).SingleOrDefault();
            if (projection == null)
            {
                projection = new TProjection();
                _keys.Map(@event, projection);
                _mappers.Map(@event, projection);
                store.Insert(projection);
            }
            else
            {
                _mappers.Map(@event, projection);
                store.Update(projection);
            }
        }
    }
}