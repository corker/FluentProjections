using System.Collections.Generic;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class UpdateFluentProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent>
    {
        private readonly ProjectionFilters<TEvent> _filters;
        private readonly EventMappers<TEvent, TProjection> _mappers;

        public UpdateFluentProjectionEventHandler(ProjectionFilters<TEvent> filters,
            EventMappers<TEvent, TProjection> mappers)
        {
            _filters = filters;
            _mappers = mappers;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
        {
            FluentProjectionFilterValues filterValues = _filters.GetValues(@event);
            IEnumerable<TProjection> projections = store.Read<TProjection>(filterValues);
            foreach (TProjection projection in projections)
            {
                _mappers.Map(@event, projection);
                store.Update(projection);
            }
        }
    }
}