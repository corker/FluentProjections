using System.Collections.Generic;

namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Mappers<TEvent, TProjection>
    {
        private readonly IEnumerable<Mapper<TEvent, TProjection>> _mappers;

        public Mappers(IEnumerable<Mapper<TEvent, TProjection>> mappers)
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