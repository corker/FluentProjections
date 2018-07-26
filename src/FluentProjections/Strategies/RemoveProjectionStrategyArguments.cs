using System.Collections.Generic;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class RemoveProjectionStrategyArguments<TMessage, TProjection> : IRegisterFilters<TMessage, TProjection>
    {
        private readonly List<Filter<TMessage>> _filters;

        public RemoveProjectionStrategyArguments()
        {
            _filters = new List<Filter<TMessage>>();
        }

        public void Register(Filter<TMessage> filter)
        {
            _filters.Add(filter);
        }

        public Filters<TMessage> Filters
        {
            get { return new Filters<TMessage>(_filters); }
        }
    }
}