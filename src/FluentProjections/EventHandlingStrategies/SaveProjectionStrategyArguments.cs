using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class SaveProjectionStrategyArguments<TEvent, TProjection> : IKeysBuilder<TEvent, TProjection>
    {
        private readonly List<Key<TEvent, TProjection>> _keys;
        private readonly List<Mapper<TEvent, TProjection>> _mappers;

        public SaveProjectionStrategyArguments()
        {
            _mappers = new List<Mapper<TEvent, TProjection>>();
            _keys = new List<Key<TEvent, TProjection>>();
        }

        public void AddMapper(Mapper<TEvent, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public void AddKey(Key<TEvent, TProjection> key)
        {
            _keys.Add(key);
        }

        public Mappers<TEvent, TProjection> Mappers
        {
            get { return new Mappers<TEvent, TProjection>(_mappers); }
        }

        public Keys<TEvent, TProjection> Keys
        {
            get { return new Keys<TEvent, TProjection>(_keys); }
        }
    }
}