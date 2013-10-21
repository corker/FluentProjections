using System;
using System.Linq.Expressions;

namespace FluentProjections.EventHandlers.Arguments
{
    public interface IEventMapperBuilder<TEvent, TProjection>
    {
        IEventMapperBuilder<TEvent, TProjection> Map<TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> action);
    }
}