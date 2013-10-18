using System.Collections.Generic;

namespace FluentProjections
{
    public class FluentProjectionMappings<TEvent, TProjection>
    {
        private readonly IEnumerable<FluentProjectionMapping<TEvent, TProjection>> _mappings;

        public FluentProjectionMappings(IEnumerable<FluentProjectionMapping<TEvent, TProjection>> mappings)
        {
            _mappings = mappings;
        }

        public void Apply(TEvent @event, TProjection projection)
        {
            foreach (var mapping in _mappings)
            {
                mapping.Apply(@event, projection);
            }
        }
    }
}