using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class RemoveProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class
    {
        private readonly Filters<TEvent> _filters;

        public RemoveProjectionStrategy(Filters<TEvent> filters)
        {
            _filters = filters;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<FluentProjectionFilterValue> filterValues = _filters.GetValues(@event);
            store.Remove<TProjection>(filterValues);
        }
    }
}