using System;
using System.Linq.Expressions;

namespace FluentProjections
{
    public class FluentProjectionArgumentsBuilder<TEvent, TProjection> :
        IFluentProjectionMappingsBuilder<TEvent, TProjection>
    {
        public IFluentProjectionMappingsBuilder<TEvent, TProjection> Map<TValue>(
            Expression<Func<TProjection, TValue>> projectionPropertyExpression,
            Expression<Func<TEvent, TValue>> eventPropertyExpression)
        {
            return this;
        }

        public FluentProjectionArgumentsBuilder<TEvent, TProjection> FilterBy<TValue>(
            Expression<Func<TProjection, TValue>> projectionPropertyExpression,
            Expression<Func<TEvent, TValue>> eventPropertyExpression)
        {
            return this;
        }

        public FluentProjectionArguments<TEvent, TProjection> Build()
        {
            return new FluentProjectionArguments<TEvent, TProjection>();
        }
    }
}