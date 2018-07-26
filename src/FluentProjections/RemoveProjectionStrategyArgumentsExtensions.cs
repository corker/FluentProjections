using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.Strategies;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections
{
    public static class RemoveProjectionStrategyArgumentsExtensions
    {
        /// <summary>
        ///     Remove projections that match a filter.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from a message</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static RemoveProjectionStrategyArguments<TMessage, TProjection> WhenEqual<TMessage, TProjection, TValue>(
            this RemoveProjectionStrategyArguments<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TMessage, TValue> getValue)
        {
            source.Register(Filter<TMessage>.Create(projectionProperty, getValue));
            return source;
        }

        /// <summary>
        ///     Remove projections that match a filter by a property from a message with the same name as in a projection.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static RemoveProjectionStrategyArguments<TMessage, TProjection> WhenEqual<TMessage, TProjection, TValue>(
            this RemoveProjectionStrategyArguments<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetMessagePropertyInfo<TMessage, TProjection, TValue>(projectionProperty);
            Func<TMessage, TValue> getValue = e => ReflectionHelpers.GetPropertyValue<TMessage, TValue>(e, propertyInfo);
            source.Register(Filter<TMessage>.Create(projectionProperty, getValue));
            return source;
        }

        /// <summary>
        ///     Remove projections that match a value.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="value">A value to filter by</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static RemoveProjectionStrategyArguments<TMessage, TProjection> WhenEqual<TMessage, TProjection, TValue>(
            this RemoveProjectionStrategyArguments<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            TValue value)
        {
            source.Register(Filter<TMessage>.Create(projectionProperty, value));
            return source;
        }
    }
}