﻿using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class UpdateProjectionStrategyArguments<TEvent, TProjection> : IRegisterFilters<TEvent, TProjection>, IRegisterMappers<TEvent, TProjection>
    {
        private readonly List<Filter<TEvent>> _filters;
        private readonly List<Mapper<TEvent, TProjection>> _mappers;

        public UpdateProjectionStrategyArguments()
        {
            _mappers = new List<Mapper<TEvent, TProjection>>();
            _filters = new List<Filter<TEvent>>();
        }

        public void Register(Mapper<TEvent, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public void Register(Filter<TEvent> filter)
        {
            _filters.Add(filter);
        }

        public Filters<TEvent> Filters
        {
            get { return new Filters<TEvent>(_filters); }
        }

        public Mappers<TEvent, TProjection> Mappers
        {
            get { return new Mappers<TEvent, TProjection>(_mappers); }
        }
    }
}