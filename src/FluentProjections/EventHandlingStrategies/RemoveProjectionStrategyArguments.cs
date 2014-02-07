using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class RemoveProjectionStrategyArguments<TEvent, TProjection> : IFiltersBuilder<TEvent, TProjection>
    {
        private readonly List<Filter<TEvent>> _filters;

        public RemoveProjectionStrategyArguments()
        {
            _filters = new List<Filter<TEvent>>();
        }

        public void AddFilter(Filter<TEvent> filter)
        {
            _filters.Add(filter);
        }

        public Filters<TEvent> Filters
        {
            get { return new Filters<TEvent>(_filters); }
        }
    }
}