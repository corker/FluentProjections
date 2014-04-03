using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.EventHandlingStrategies;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections
{
    public static class UpdateProjectionStrategyArgumentsExtensions
    {
        /// <summary>
        ///     Update projections that match a filter.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static UpdateProjectionStrategyArguments<TEvent, TProjection> WhenEqual<TEvent, TProjection, TValue>(
            this UpdateProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            source.AddFilter(Filter<TEvent>.Create(projectionProperty, getValue));
            return source;
        }

        /// <summary>
        ///     Update projections that match a filter by a property from an event with the same name as in a projection.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static UpdateProjectionStrategyArguments<TEvent, TProjection> WhenEqual<TEvent, TProjection, TValue>(
            this UpdateProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            Func<TEvent, TValue> getValue = e => ReflectionHelpers.GetPropertyValue<TEvent, TValue>(e, propertyInfo);
            source.AddFilter(Filter<TEvent>.Create(projectionProperty, getValue));
            return source;
        }

        /// <summary>
        ///     Update projections that match a filter by a provided value.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="value">A value to filter by</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static UpdateProjectionStrategyArguments<TEvent, TProjection> WhenEqual<TEvent, TProjection, TValue>(
            this UpdateProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            TValue value)
        {
            source.AddFilter(Filter<TEvent>.Create(projectionProperty, value));
            return source;
        }
    }
}