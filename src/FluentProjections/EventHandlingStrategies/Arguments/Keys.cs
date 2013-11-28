using System.Collections.Generic;
using System.Linq;

namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Keys<TEvent, TProjection>
    {
        private readonly Filters<TEvent> _filters;
        private readonly Mappers<TEvent, TProjection> _mappers;

        public Keys(IEnumerable<Key<TEvent, TProjection>> keys)
        {
            var projectionKeys = keys.ToList();
            _filters = new Filters<TEvent>(projectionKeys.Select(x => x.Filter).ToList());
            _mappers = new Mappers<TEvent, TProjection>(projectionKeys.Select(x => x.Mapper).ToList());
        }

        public IEnumerable<FluentProjectionFilterValue> GetValues(TEvent @event)
        {
            return _filters.GetValues(@event);
        }

        public void Map(TEvent @event, TProjection projection)
        {
            _mappers.Map(@event, projection);
        }
    }
}