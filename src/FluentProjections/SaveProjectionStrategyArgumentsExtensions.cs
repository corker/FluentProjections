using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.EventHandlingStrategies;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections
{
    public static class SaveProjectionStrategyArgumentsExtensions
    {
        /// <summary>
        ///     Update projection that matches a key or insert a new projection when no matching projection found.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static SaveProjectionStrategyArguments<TEvent, TProjection> WithKey<TEvent, TProjection, TValue>(
            this SaveProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            Action<TProjection, TValue> action = ReflectionHelpers.CreateSetOperation(projectionProperty);
            Mapper<TEvent, TProjection> mapper = Mapper<TEvent, TProjection>.Create((e, p) => action(p, getValue(e)));
            Filter<TEvent> filter = Filter<TEvent>.Create(projectionProperty, getValue);
            Key<TEvent, TProjection> key = Key<TEvent, TProjection>.Create(filter, mapper);
            source.Register(key);
            return source;
        }

        /// <summary>
        ///     Update projection that matches a key or insert a new projection when no matching projection found by a property
        ///     from an event with the same name as in a projection.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static SaveProjectionStrategyArguments<TEvent, TProjection> WithKey<TEvent, TProjection, TValue>(
            this SaveProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            Func<TEvent, TValue> getValue = e => ReflectionHelpers.GetPropertyValue<TEvent, TValue>(e, propertyInfo);
            Action<TProjection, TValue> action = ReflectionHelpers.CreateSetOperation(projectionProperty);
            Mapper<TEvent, TProjection> mapper = Mapper<TEvent, TProjection>.Create((e, p) => action(p, getValue(e)));
            Filter<TEvent> filter = Filter<TEvent>.Create(projectionProperty, getValue);
            Key<TEvent, TProjection> key = Key<TEvent, TProjection>.Create(filter, mapper);
            source.Register(key);
            return source;
        }

        /// <summary>
        ///     Update projection that matches a key or insert a new projection when no matching projection found by a provided
        ///     value.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="value"></param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static SaveProjectionStrategyArguments<TEvent, TProjection> WithKey<TEvent, TProjection, TValue>(
            this SaveProjectionStrategyArguments<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            TValue value)
        {
            Action<TProjection, TValue> action = ReflectionHelpers.CreateSetOperation(projectionProperty);
            Mapper<TEvent, TProjection> mapper = Mapper<TEvent, TProjection>.Create((e, p) => action(p, value));
            Filter<TEvent> filter = Filter<TEvent>.Create(projectionProperty, value);
            Key<TEvent, TProjection> key = Key<TEvent, TProjection>.Create(filter, mapper);
            source.Register(key);
            return source;
        }
    }
}