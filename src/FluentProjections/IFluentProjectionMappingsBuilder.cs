using System;
using System.Linq.Expressions;

namespace FluentProjections
{
    public interface IFluentProjectionMappingsBuilder<TEvent, TProjection>
    {
        IFluentProjectionMappingsBuilder<TEvent, TProjection> Map<TValue>(
            Expression<Func<TProjection, TValue>> projectionPropertyExpression,
            Expression<Func<TEvent, TValue>> eventPropertyExpression);
    }
}