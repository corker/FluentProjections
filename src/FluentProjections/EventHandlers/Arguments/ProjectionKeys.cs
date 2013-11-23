using System.Collections.Generic;
using System.Linq;

namespace FluentProjections.EventHandlers.Arguments
{
    public class ProjectionKeys<TEvent, TProjection>
    {
        private readonly ProjectionFilters<TEvent> _filters;
        private readonly EventMappers<TEvent, TProjection> _mappers;

        public ProjectionKeys(IEnumerable<ProjectionKey<TEvent, TProjection>> keys)
        {
            var projectionKeys = keys.ToList();
            _filters = new ProjectionFilters<TEvent>(projectionKeys.Select(x => x.Filter).ToList());
            _mappers = new EventMappers<TEvent, TProjection>(projectionKeys.Select(x => x.Mapper).ToList());
        }

        public FluentProjectionFilterValues GetValues(TEvent @event)
        {
            return _filters.GetValues(@event);
        }

        public void Map(TEvent @event, TProjection projection)
        {
            _mappers.Map(@event, projection);
        }
    }
}