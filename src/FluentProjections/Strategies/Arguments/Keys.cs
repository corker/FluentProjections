using System.Collections.Generic;
using System.Linq;
using FluentProjections.Persistence;

namespace FluentProjections.Strategies.Arguments
{
    public class Keys<TMessage, TProjection>
    {
        private readonly Filters<TMessage> _filters;
        private readonly Mappers<TMessage, TProjection> _mappers;

        public Keys(IEnumerable<Key<TMessage, TProjection>> keys)
        {
            var projectionKeys = keys.ToList();
            _filters = new Filters<TMessage>(projectionKeys.Select(x => x.Filter).ToList());
            _mappers = new Mappers<TMessage, TProjection>(projectionKeys.Select(x => x.Mapper).ToList());
        }

        public IEnumerable<FilterValue> GetValues(TMessage message)
        {
            return _filters.GetValues(message);
        }

        public void Map(TMessage message, TProjection projection)
        {
            _mappers.Map(message, projection);
        }
    }
}