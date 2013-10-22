using System;
using System.Linq.Expressions;

namespace FluentProjections.EventHandlers.Arguments
{
    public interface IEventMapperBuilder<TEvent, TProjection>
    {
        IEventMapperBuilder<TEvent, TProjection> Map<TValue>(Expression<Func<TProjection, TValue>> projectionProperty, Func<TEvent, TValue> action);

        IEventMapperBuilder<TEvent, TProjection> Map<TValue>(Expression<Func<TProjection, TValue>> projectionProperty);

        IEventMapperBuilder<TEvent, TProjection> Add<TValue>(Expression<Func<TProjection, TValue>> projectionProperty, Func<TEvent, TValue> getValue)
            where TValue : IComparable<TValue>;

        IEventMapperBuilder<TEvent, TProjection> Add<TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>;

        IEventMapperBuilder<TEvent, TProjection> Substract<TValue>(Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
            where TValue : IComparable<TValue>;

        IEventMapperBuilder<TEvent, TProjection> Substract<TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>;

        IEventMapperBuilder<TEvent, TProjection> Increment(Expression<Func<TProjection, long>> projectionProperty);
        IEventMapperBuilder<TEvent, TProjection> Decrement(Expression<Func<TProjection, long>> projectionProperty);
        IEventMapperBuilder<TEvent, TProjection> Do(Action<TEvent, TProjection> action);
    }
}