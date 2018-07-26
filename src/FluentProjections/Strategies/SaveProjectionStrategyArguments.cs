using System.Collections.Generic;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class SaveProjectionStrategyArguments<TMessage, TProjection> : IRegisterKeys<TMessage, TProjection>
    {
        private readonly List<Key<TMessage, TProjection>> _keys;
        private readonly List<Mapper<TMessage, TProjection>> _mappers;

        public SaveProjectionStrategyArguments()
        {
            _mappers = new List<Mapper<TMessage, TProjection>>();
            _keys = new List<Key<TMessage, TProjection>>();
        }

        public void Register(Mapper<TMessage, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public void Register(Key<TMessage, TProjection> key)
        {
            _keys.Add(key);
        }

        public Mappers<TMessage, TProjection> Mappers
        {
            get { return new Mappers<TMessage, TProjection>(_mappers); }
        }

        public Keys<TMessage, TProjection> Keys
        {
            get { return new Keys<TMessage, TProjection>(_keys); }
        }
    }
}