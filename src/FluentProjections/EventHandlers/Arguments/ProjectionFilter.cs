using System;
using System.Reflection;

namespace FluentProjections.EventHandlers.Arguments
{
    public class ProjectionFilter<TEvent>
    {
        private readonly PropertyInfo _property;
        private readonly Func<TEvent, object> _getValue;

        public ProjectionFilter(PropertyInfo property, Func<TEvent, object> getValue)
        {
            _property = property;
            _getValue = getValue;
        }

        public FluentProjectionFilterValue GetValue(TEvent @event)
        {
            return new FluentProjectionFilterValue(_property, _getValue(@event));
        }
    }
}