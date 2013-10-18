using System;

namespace FluentProjections
{
    public class FluentProjectionMapping<TEvent, TProjection>
    {
        private readonly Action<TEvent, TProjection> _action;

        public FluentProjectionMapping(Action<TEvent, TProjection> action)
        {
            _action = action;
        }

        public void Apply(TEvent @event, TProjection projection)
        {
            _action(@event, projection);
        }
    }
}