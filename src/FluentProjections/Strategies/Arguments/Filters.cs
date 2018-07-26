using System.Collections.Generic;
using System.Linq;
using FluentProjections.Logging.Generic;
using FluentProjections.Persistence;

namespace FluentProjections.Strategies.Arguments
{
    public class Filters<TMessage>
    {
        private readonly List<Filter<TMessage>> _filters;

        public Filters(List<Filter<TMessage>> filters)
        {
            _filters = filters;
        }

        public IEnumerable<FilterValue> GetValues(TMessage message)
        {
            return _filters.Select(x => x.GetValue(message)).ToList();
        }
    }
}