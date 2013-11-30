﻿using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class AddNewProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class, new()
    {
        private readonly Mappers<TEvent, TProjection> _mappers;

        public AddNewProjectionStrategy(Mappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            var projection = new TProjection();
            _mappers.Map(@event, projection);
            store.Insert(projection);
        }
    }
}