using System.Collections.Generic;
using System.Linq;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class SaveProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class, new()
    {
        private readonly Keys<TEvent, TProjection> _keys;
        private readonly Mappers<TEvent, TProjection> _mappers;

        public SaveProjectionStrategy(
            Keys<TEvent, TProjection> keys,
            Mappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
            _keys = keys;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<FluentProjectionFilterValue> filterValues = _keys.GetValues(@event);
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