using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.EventHandlingStrategies;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections
{
    public static class RemoveProjectionStrategyArgumentsExtensions
    {
        /// <summary>
        /// Remove projections that match a filter.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static RemoveProjectionStrategyArguments<TEvent, TProjection> FilterBy<TEvent, TProjection, TValue>(
            this RemoveProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            var memberExpression = (MemberExpression)projectionProperty.Body;
            var property = (PropertyInfo)memberExpression.Member;
            source.AddFilter(new Filter<TEvent>(property, e => getValue(e)));
            return source;
        }
    }
}