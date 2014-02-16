using System;

namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Mapper<TEvent, TProjection>
    {
        private readonly Action<TEvent, TProjection> _action;

        private Mapper(Action<TEvent, TProjection> action)
        {
            _action = action;
        }

        public void Apply(TEvent @event, TProjection projection)
        {
            _action(@event, projection);
        }

        public static Mapper<TEvent, TProjection> Create(Action<TEvent, TProjection> action)
        {
            return new Mapper<TEvent, TProjection>(action);
        }
    }
}