using System.Collections.Generic;

namespace FluentProjections.EventHandlers.Arguments
{
    public class EventMappers<TEvent, TProjection>
    {
        private readonly IEnumerable<EventMapper<TEvent, TProjection>> _mappers;

        public EventMappers(IEnumerable<EventMapper<TEvent, TProjection>> mappers)
        {
            _mappers = mappers;
        }

        public void Map(TEvent @event, TProjection projection)
        {
            foreach (var mapper in _mappers)
            {
                mapper.Apply(@event, projection);
            }
        }
    }
}