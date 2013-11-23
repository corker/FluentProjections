using System.Collections.Generic;
using System.Linq;

namespace FluentProjections.EventHandlers.Arguments
{
    public class ProjectionFilters<TEvent>
    {
        private readonly List<ProjectionFilter<TEvent>> _filters;

        public ProjectionFilters(List<ProjectionFilter<TEvent>> filters)
        {
            _filters = filters;
        }

        public FluentProjectionFilterValues GetValues(TEvent @event)
        {
            List<FluentProjectionFilterValue> values = _filters.Select(x => x.GetValue(@event)).ToList();
            return new FluentProjectionFilterValues(values);
        }
    }
}