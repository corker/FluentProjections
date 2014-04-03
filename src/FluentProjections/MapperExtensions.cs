using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections
{
    public static class MapperExtensions
    {
        /// <summary>
        ///     Do an action with a projection using an event
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="action">An action to perform on projection</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Do<TEvent, TProjection>(
            this IMappersBuilder<TEvent, TProjection> source,
            Action<TEvent, TProjection> action)
        {
            source.AddMapper(Mapper<TEvent, TProjection>.Create(action));
            return source;
        }

        /// <summary>
        ///     Map a property from an event to a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Map<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            return source.Do(ReflectionHelpers.CreateSetOperation(projectionProperty), getValue);
        }

        /// <summary>
        ///     Map a property from an event with the same name as in a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Map<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            return source.Map(projectionProperty, e => ReflectionHelpers.GetPropertyValue<TEvent, TValue>(e, propertyInfo));
        }

        /// <summary>
        ///     Add a property value from an event to a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Add<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue) where TValue : IComparable<TValue>
        {
            return source.Do(ReflectionHelpers.CreateBinaryOperation(ExpressionType.Add, projectionProperty), getValue);
        }

        /// <summary>
        ///     Add a property value from an event with the same name as in a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Add<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            return source.Add(projectionProperty, e => ReflectionHelpers.GetPropertyValue<TEvent, TValue>(e, propertyInfo));
        }

        /// <summary>
        ///     Increment a property value in a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Increment<TEvent, TProjection>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, long>> projectionProperty)
        {
            return source.Add(projectionProperty, e => 1);
        }

        /// <summary>
        ///     Substract an event property value from a projection propetry
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Substract<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue) where TValue : IComparable<TValue>
        {
            return source.Do(ReflectionHelpers.CreateBinaryOperation(ExpressionType.Subtract, projectionProperty), getValue);
        }

        /// <summary>
        ///     Substract an event property value from a property with the same name as in a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Substract<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            return source.Substract(projectionProperty, e => ReflectionHelpers.GetPropertyValue<TEvent, TValue>(e, propertyInfo));
        }

        /// <summary>
        ///     Decrement a property value in a projection
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Decrement<TEvent, TProjection>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, long>> projectionProperty)
        {
            return source.Substract(projectionProperty, e => 1);
        }

        /// <summary>
        ///     Set a property value
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="value">A new value for the property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMappersBuilder<TEvent, TProjection> Set<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty, 
            TValue value)
        {
            return source.Do((e, p) => ReflectionHelpers.CreateSetOperation(projectionProperty)(p, value));
        }

        private static IMappersBuilder<TEvent, TProjection> Do<TEvent, TProjection, TValue>(
            this IMappersBuilder<TEvent, TProjection> source,
            Action<TProjection, TValue> action,
            Func<TEvent, TValue> getValue)
        {
            return source.Do((e, p) => action(p, getValue(e)));
        }
    }
}