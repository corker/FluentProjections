using System.Collections.Generic;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class UpdateProjectionStrategyArguments<TMessage, TProjection> : IRegisterFilters<TMessage, TProjection>, IRegisterMappers<TMessage, TProjection>
    {
        private readonly List<Filter<TMessage>> _filters;
        private readonly List<Mapper<TMessage, TProjection>> _mappers;

        public UpdateProjectionStrategyArguments()
        {
            _mappers = new List<Mapper<TMessage, TProjection>>();
            _filters = new List<Filter<TMessage>>();
        }

        public void Register(Mapper<TMessage, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public void Register(Filter<TMessage> filter)
        {
            _filters.Add(filter);
        }

        public Filters<TMessage> Filters
        {
            get { return new Filters<TMessage>(_filters); }
        }

        public Mappers<TMessage, TProjection> Mappers
        {
            get { return new Mappers<TMessage, TProjection>(_mappers); }
        }
    }
}