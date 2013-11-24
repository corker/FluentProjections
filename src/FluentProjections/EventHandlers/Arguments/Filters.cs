using System.Collections.Generic;
using System.Linq;

namespace FluentProjections.EventHandlers.Arguments
{
    public class Filters<TEvent>
    {
        private readonly List<Filter<TEvent>> _filters;

        public Filters(List<Filter<TEvent>> filters)
        {
            _filters = filters;
        }

        public IEnumerable<FluentProjectionFilterValue> GetValues(TEvent @event)
        {
            return _filters.Select(x => x.GetValue(@event)).ToList();
        }
    }
}