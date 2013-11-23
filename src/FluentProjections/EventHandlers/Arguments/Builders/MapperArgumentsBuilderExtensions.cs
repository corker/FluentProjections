﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentProjections.EventHandlers.Arguments.Builders
{
    public static class MapperArgumentsBuilderExtensions
    {
        /// <summary>
        /// Do an <param name="action"></param> with <typeparam name="TProjection"></typeparam> using <typeparam name="TEvent"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="action">An action to perform on projection</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Do<TEvent, TProjection>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Action<TEvent, TProjection> action)
        {
            source.AddMapper(new EventMapper<TEvent, TProjection>(action));
            return source;
        }

        /// <summary>
        /// Map a property from <typeparam name="TEvent"></typeparam> to a <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Map<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            return source.Do(CreateSetOperation(projectionProperty), getValue);
        }

        /// <summary>
        /// Map a property from <typeparam name="TEvent"></typeparam> with the same name as in <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Map<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            return source.Map(projectionProperty, e => GetPropertyValue<TEvent, TValue>(e, propertyInfo));
        }

        /// <summary>
        /// Add a property value from <typeparam name="TEvent"></typeparam> to a <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Add<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue) where TValue : IComparable<TValue>
        {
            return source.Do(CreateBinaryOperation(ExpressionType.Add, projectionProperty), getValue);
        }

        /// <summary>
        /// Add a property value from <typeparam name="TEvent"></typeparam> with the same name as in <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Add<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            return source.Add(projectionProperty, e => GetPropertyValue<TEvent, TValue>(e, propertyInfo));
        }

        /// <summary>
        /// Increment a property value in <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Increment<TEvent, TProjection>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, long>> projectionProperty)
        {
            return source.Add(projectionProperty, e => 1);
        }

        /// <summary>
        /// Substract a property value from <typeparam name="TEvent"></typeparam> from a <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Substract<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue) where TValue : IComparable<TValue>
        {
            return source.Do(CreateBinaryOperation(ExpressionType.Subtract, projectionProperty), getValue);
        }

        /// <summary>
        /// Substract a property value from <typeparam name="TEvent"></typeparam> with the same name as in <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Substract<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = GetEventPropertyInfo<TEvent, TProjection, TValue>(projectionProperty);
            return source.Substract(projectionProperty, e => GetPropertyValue<TEvent, TValue>(e, propertyInfo));
        }

        /// <summary>
        /// Decrement a property value in <typeparam name="TProjection"></typeparam>
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IMapperArgumentsBuilder<TEvent, TProjection> Decrement<TEvent, TProjection>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, long>> projectionProperty)
        {
            return source.Substract(projectionProperty, e => 1);
        }

        private static IMapperArgumentsBuilder<TEvent, TProjection> Do<TEvent, TProjection, TValue>(
            this IMapperArgumentsBuilder<TEvent, TProjection> source,
            Action<TProjection, TValue> action,
            Func<TEvent, TValue> getValue)
        {
            return source.Do((e, p) => action(p, getValue(e)));
        }

        private static Action<TProjection, TValue> CreateBinaryOperation<TProjection, TValue>(
            ExpressionType type,
            Expression<Func<TProjection, TValue>> expression)
            where TValue : IComparable<TValue>
        {
            PropertyInfo property = GetPropertyInfo(expression);
            MethodInfo getMethod = property.GetGetMethod();
            MethodInfo setMethod = property.GetSetMethod();

            ParameterExpression parameterProjection = Expression.Parameter(typeof (TProjection), "projection");
            ParameterExpression parameterValue = Expression.Parameter(typeof (TValue), "value");

            Expression<Action<TProjection, TValue>> lambda =
                Expression.Lambda<Action<TProjection, TValue>>(
                    Expression.Call(parameterProjection, setMethod,
                        Expression.MakeBinary(type, Expression.Call(parameterProjection, getMethod), parameterValue)),
                    parameterProjection,
                    parameterValue
                    );

            return lambda.Compile();
        }

        private static PropertyInfo GetEventPropertyInfo<TEvent, TProjection, TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = typeof (TEvent).GetProperty(GetPropertyInfo(projectionProperty).Name);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException("projectionProperty", "No associated event property found.");
            }
            return propertyInfo;
        }

        private static PropertyInfo GetPropertyInfo<TProjection, TValue>(Expression<Func<TProjection, TValue>> expression)
        {
            return (PropertyInfo) ((MemberExpression) expression.Body).Member;
        }

        private static TValue GetPropertyValue<TEvent, TValue>(TEvent @event, PropertyInfo propertyInfo)
        {
            return (TValue) propertyInfo.GetValue(@event, new object[0]);
        }

        private static Action<TProjection, TValue> CreateSetOperation<TProjection, TValue>(Expression<Func<TProjection, TValue>> expression)
        {
            PropertyInfo property = GetPropertyInfo(expression);
            MethodInfo setMethod = property.GetSetMethod();

            ParameterExpression parameterProjection = Expression.Parameter(typeof (TProjection), "projection");
            ParameterExpression parameterValue = Expression.Parameter(typeof (TValue), "value");

            Expression<Action<TProjection, TValue>> lambda =
                Expression.Lambda<Action<TProjection, TValue>>(
                    Expression.Call(parameterProjection, setMethod, parameterValue),
                    parameterProjection,
                    parameterValue
                    );

            return lambda.Compile();
        }
    }
}