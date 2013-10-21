using System;

namespace FluentProjections.EventHandlers.Arguments
{
    public class EventMapper<TEvent, TProjection>
    {
        private readonly Action<TEvent, TProjection> _action;

        public EventMapper(Action<TEvent, TProjection> action)
        {
            _action = action;
        }

        public void Apply(TEvent @event, TProjection projection)
        {
            _action(@event, projection);
        }
    }
}