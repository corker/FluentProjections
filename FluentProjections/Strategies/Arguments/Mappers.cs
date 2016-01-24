using System.Collections.Generic;

namespace FluentProjections.Strategies.Arguments
{
    public class Mappers<TMessage, TProjection>
    {
        private readonly IEnumerable<Mapper<TMessage, TProjection>> _mappers;

        public Mappers(IEnumerable<Mapper<TMessage, TProjection>> mappers)
        {
            _mappers = mappers;
        }

        public void Map(TMessage message, TProjection projection)
        {
            foreach (var mapper in _mappers)
            {
                mapper.Apply(message, projection);
            }
        }
    }
}