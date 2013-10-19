using System.Collections.Generic;
using System.Linq;

namespace FluentProjections
{
    public class FluentProjectionFilters<TEvent>
    {
        private readonly List<FluentProjectionFilter<TEvent>> _filters;

        public FluentProjectionFilters(List<FluentProjectionFilter<TEvent>> filters)
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