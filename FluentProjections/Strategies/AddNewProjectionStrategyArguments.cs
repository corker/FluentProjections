using System.Collections.Generic;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class AddNewProjectionStrategyArguments<TMessage, TProjection> : IRegisterMappers<TMessage, TProjection>
    {
        private readonly List<Mapper<TMessage, TProjection>> _mappers;

        public AddNewProjectionStrategyArguments()
        {
            _mappers = new List<Mapper<TMessage, TProjection>>();
        }

        public void Register(Mapper<TMessage, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public Mappers<TMessage, TProjection> Mappers
        {
            get { return new Mappers<TMessage, TProjection>(_mappers); }
        }
    }
}