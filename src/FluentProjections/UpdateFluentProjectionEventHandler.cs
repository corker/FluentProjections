using System.Collections.Generic;

namespace FluentProjections
{
    public class UpdateFluentProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent, TProjection>
    {
        private readonly FluentProjectionFilters<TEvent> _filters;
        private readonly FluentProjectionMappings<TEvent, TProjection> _mappings;

        public UpdateFluentProjectionEventHandler(FluentProjectionFilters<TEvent> filters,
            FluentProjectionMappings<TEvent, TProjection> mappings)
        {
            _filters = filters;
            _mappings = mappings;
        }

        public void Handle(TEvent @event, IFluentProjectionStore<TProjection> store)
        {
            FluentProjectionFilterValues filterValues = _filters.GetValues(@event);
            IEnumerable<TProjection> projections = store.Read(filterValues);
            foreach (TProjection projection in projections)
            {
                _mappings.Apply(@event, projection);
                store.Update(projection);
            }
        }
    }
}