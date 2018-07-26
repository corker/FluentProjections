using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.Strategies;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections
{
    public static class SaveProjectionStrategyArgumentsExtensions
    {
        /// <summary>
        ///     Update projection that matches a key or insert a new projection when no matching projection found.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from a message</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static SaveProjectionStrategyArguments<TMessage, TProjection> WithKey<TMessage, TProjection, TValue>(
            this SaveProjectionStrategyArguments<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TMessage, TValue> getValue)
        {
            Action<TProjection, TValue> action = ReflectionHelpers.CreateSetOperation(projectionProperty);
            Mapper<TMessage, TProjection> mapper = Mapper<TMessage, TProjection>.Create((e, p) => action(p, getValue(e)));
            Filter<TMessage> filter = Filter<TMessage>.Create(projectionProperty, getValue);
            Key<TMessage, TProjection> key = Key<TMessage, TProjection>.Create(filter, mapper);
            source.Register(key);
            return source;
        }

        /// <summary>
        ///     Update projection that matches a key or insert a new projection when no matching projection found by a property
        ///     from a message with the same name as in a projection.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static SaveProjectionStrategyArguments<TMessage, TProjection> WithKey<TMessage, TProjection, TValue>(
            this SaveProjectionStrategyArguments<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetMessagePropertyInfo<TMessage, TProjection, TValue>(projectionProperty);
            Func<TMessage, TValue> getValue = e => ReflectionHelpers.GetPropertyValue<TMessage, TValue>(e, propertyInfo);
            Action<TProjection, TValue> action = ReflectionHelpers.CreateSetOperation(projectionProperty);
            Mapper<TMessage, TProjection> mapper = Mapper<TMessage, TProjection>.Create((e, p) => action(p, getValue(e)));
            Filter<TMessage> filter = Filter<TMessage>.Create(projectionProperty, getValue);
            Key<TMessage, TProjection> key = Key<TMessage, TProjection>.Create(filter, mapper);
            source.Register(key);
            return source;
        }

        /// <summary>
        ///     Update projection that matches a key or insert a new projection when no matching projection found by a provided
        ///     value.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="value"></param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static SaveProjectionStrategyArguments<TMessage, TProjection> WithKey<TMessage, TProjection, TValue>(
            this SaveProjectionStrategyArguments<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            TValue value)
        {
            Action<TProjection, TValue> action = ReflectionHelpers.CreateSetOperation(projectionProperty);
            Mapper<TMessage, TProjection> mapper = Mapper<TMessage, TProjection>.Create((e, p) => action(p, value));
            Filter<TMessage> filter = Filter<TMessage>.Create(projectionProperty, value);
            Key<TMessage, TProjection> key = Key<TMessage, TProjection>.Create(filter, mapper);
            source.Register(key);
            return source;
        }
    }
}