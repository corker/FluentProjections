using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentProjections.EventHandlers.Arguments
{
    public static class ArgumentsBuilderFilterExtensions
    {
        public static ArgumentsBuilder<TEvent, TProjection> FilterBy<TEvent, TProjection, TValue>(
            this ArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, object> getValue)
        {
            var memberExpression = (MemberExpression)projectionProperty.Body;
            var property = (PropertyInfo)memberExpression.Member;
            source.AddFilter(new ProjectionFilter<TEvent>(property, getValue));
            return source;
        }
    }
}