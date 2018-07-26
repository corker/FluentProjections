using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections
{
    public static class MapperExtensions
    {
        /// <summary>
        ///     Do an action with a projection using a message
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="action">An action to perform on projection</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Do<TMessage, TProjection>(
            this IRegisterMappers<TMessage, TProjection> source,
            Action<TMessage, TProjection> action)
        {
            source.Register(Mapper<TMessage, TProjection>.Create(action));
            return source;
        }

        /// <summary>
        ///     Map a property from a message to a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from a message</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Map<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TMessage, TValue> getValue)
        {
            return source.Do(ReflectionHelpers.CreateSetOperation(projectionProperty), getValue);
        }

        /// <summary>
        ///     Map a property from a message with the same name as in a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Map<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetMessagePropertyInfo<TMessage, TProjection, TValue>(projectionProperty);
            return source.Map(projectionProperty, e => ReflectionHelpers.GetPropertyValue<TMessage, TValue>(e, propertyInfo));
        }

        /// <summary>
        ///     Add a property value from a message to a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from a message</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Add<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TMessage, TValue> getValue) where TValue : IComparable<TValue>
        {
            return source.Do(ReflectionHelpers.CreateBinaryOperation(ExpressionType.Add, projectionProperty), getValue);
        }

        /// <summary>
        ///     Add a property value from a message with the same name as in a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Add<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetMessagePropertyInfo<TMessage, TProjection, TValue>(projectionProperty);
            return source.Add(projectionProperty, e => ReflectionHelpers.GetPropertyValue<TMessage, TValue>(e, propertyInfo));
        }

        /// <summary>
        ///     Increment a property value in a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Increment<TMessage, TProjection>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, long>> projectionProperty)
        {
            return source.Add(projectionProperty, e => 1);
        }

        /// <summary>
        ///     Substract a message property value from a projection propetry
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from a message</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Substract<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TMessage, TValue> getValue) where TValue : IComparable<TValue>
        {
            return source.Do(ReflectionHelpers.CreateBinaryOperation(ExpressionType.Subtract, projectionProperty), getValue);
        }

        /// <summary>
        ///     Substract a message property value from a property with the same name as in a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Substract<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = ReflectionHelpers.GetMessagePropertyInfo<TMessage, TProjection, TValue>(projectionProperty);
            return source.Substract(projectionProperty, e => ReflectionHelpers.GetPropertyValue<TMessage, TValue>(e, propertyInfo));
        }

        /// <summary>
        ///     Decrement a property value in a projection
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Decrement<TMessage, TProjection>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, long>> projectionProperty)
        {
            return source.Substract(projectionProperty, e => 1);
        }

        /// <summary>
        ///     Set a property value
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="value">A new value for the property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TMessage, TProjection> Set<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty, 
            TValue value)
        {
            return source.Do((e, p) => ReflectionHelpers.CreateSetOperation(projectionProperty)(p, value));
        }

        private static IRegisterMappers<TMessage, TProjection> Do<TMessage, TProjection, TValue>(
            this IRegisterMappers<TMessage, TProjection> source,
            Action<TProjection, TValue> action,
            Func<TMessage, TValue> getValue)
        {
            return source.Do((e, p) => action(p, getValue(e)));
        }
    }
}