﻿using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class AddNewProjectionStrategyArguments<TEvent, TProjection> : IRegisterMappers<TEvent, TProjection>
    {
        private readonly List<Mapper<TEvent, TProjection>> _mappers;

        public AddNewProjectionStrategyArguments()
        {
            _mappers = new List<Mapper<TEvent, TProjection>>();
        }

        public void Register(Mapper<TEvent, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public Mappers<TEvent, TProjection> Mappers
        {
            get { return new Mappers<TEvent, TProjection>(_mappers); }
        }
    }
}