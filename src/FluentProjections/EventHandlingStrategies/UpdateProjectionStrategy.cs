using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class UpdateProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class
    {
        private readonly Filters<TEvent> _filters;
        private readonly Mappers<TEvent, TProjection> _mappers;

        public UpdateProjectionStrategy(Filters<TEvent> filters,
            Mappers<TEvent, TProjection> mappers)
        {
            _filters = filters;
            _mappers = mappers;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<FluentProjectionFilterValue> filterValues = _filters.GetValues(@event);
            IEnumerable<TProjection> projections = store.Read<TProjection>(filterValues);
            foreach (TProjection projection in projections)
            {
                _mappers.Map(@event, projection);
                store.Update(projection);
            }
        }
    }
}