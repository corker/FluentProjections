using System.Collections.Generic;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class UpdateProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent>
        where TProjection : class
    {
        private readonly Filters<TEvent> _filters;
        private readonly Mappers<TEvent, TProjection> _mappers;

        public UpdateProjectionEventHandler(Filters<TEvent> filters,
            Mappers<TEvent, TProjection> mappers)
        {
            _filters = filters;
            _mappers = mappers;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
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