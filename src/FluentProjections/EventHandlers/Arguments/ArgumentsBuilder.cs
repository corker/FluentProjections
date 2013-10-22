using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentProjections.EventHandlers.Arguments
{
    public class ArgumentsBuilder<TEvent, TProjection> :
        IEventMapperBuilder<TEvent, TProjection>
    {
        private readonly Type _eventType;
        private readonly List<FluentProjectionFilter<TEvent>> _filters;
        private readonly List<EventMapper<TEvent, TProjection>> _mappers;

        public ArgumentsBuilder()
        {
            _filters = new List<FluentProjectionFilter<TEvent>>();
            _mappers = new List<EventMapper<TEvent, TProjection>>();
            _eventType = typeof (TEvent);
        }

        public IEventMapperBuilder<TEvent, TProjection> Do(Action<TEvent, TProjection> action)
        {
            _mappers.Add(new EventMapper<TEvent, TProjection>(action));
            return this;
        }

        public IEventMapperBuilder<TEvent, TProjection> Map<TValue>(Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            return Do(CreateSetOperation(projectionProperty), getValue);
        }

        public IEventMapperBuilder<TEvent, TProjection> Add<TValue>(Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue) where TValue : IComparable<TValue>
        {
            return Do(CreateBinaryOperation(ExpressionType.Add, projectionProperty), getValue);
        }

        public IEventMapperBuilder<TEvent, TProjection> Substract<TValue>(Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue) where TValue : IComparable<TValue>
        {
            return Do(CreateBinaryOperation(ExpressionType.Subtract, projectionProperty), getValue);
        }

        public IEventMapperBuilder<TEvent, TProjection> Map<TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = GetEventPropertyInfo(projectionProperty);
            return Map(projectionProperty, e => GetPropertyValue<TValue>(e, propertyInfo));
        }

        public IEventMapperBuilder<TEvent, TProjection> Add<TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = GetEventPropertyInfo(projectionProperty);
            return Add(projectionProperty, e => GetPropertyValue<TValue>(e, propertyInfo));
        }

        public IEventMapperBuilder<TEvent, TProjection> Substract<TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
            where TValue : IComparable<TValue>
        {
            PropertyInfo propertyInfo = GetEventPropertyInfo(projectionProperty);
            return Substract(projectionProperty, e => GetPropertyValue<TValue>(e, propertyInfo));
        }

        public IEventMapperBuilder<TEvent, TProjection> Increment(Expression<Func<TProjection, long>> projectionProperty)
        {
            return Add(projectionProperty, e => 1);
        }

        public IEventMapperBuilder<TEvent, TProjection> Decrement(Expression<Func<TProjection, long>> projectionProperty)
        {
            return Substract(projectionProperty, e => 1);
        }

        private static TValue GetPropertyValue<TValue>(TEvent @event, PropertyInfo propertyInfo)
        {
            return (TValue) propertyInfo.GetValue(@event, new object[0]);
        }

        private PropertyInfo GetEventPropertyInfo<TValue>(Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = _eventType.GetProperty(GetPropertyInfo(projectionProperty).Name);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException("projectionProperty", "No associated event property found.");
            }
            return propertyInfo;
        }

        private IEventMapperBuilder<TEvent, TProjection> Do<TValue>(Action<TProjection, TValue> action, Func<TEvent, TValue> getValue)
        {
            return Do((e, p) => action(p, getValue(e)));
        }

        private static PropertyInfo GetPropertyInfo<TValue>(Expression<Func<TProjection, TValue>> expression)
        {
            return (PropertyInfo) ((MemberExpression) expression.Body).Member;
        }

        private static Action<TProjection, TValue> CreateSetOperation<TValue>(Expression<Func<TProjection, TValue>> expression)
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

        private static Action<TProjection, TValue> CreateBinaryOperation<TValue>(ExpressionType type, Expression<Func<TProjection, TValue>> expression)
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

        public ArgumentsBuilder<TEvent, TProjection> FilterBy<TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, object> getValue)
        {
            var memberExpression = (MemberExpression) projectionProperty.Body;
            var property = (PropertyInfo) memberExpression.Member;

            _filters.Add(new FluentProjectionFilter<TEvent>(property, getValue));
            return this;
        }

        public FluentProjectionFilters<TEvent> BuildFilters()
        {
            return new FluentProjectionFilters<TEvent>(_filters);
        }

        public EventMappers<TEvent, TProjection> BuildMappers()
        {
            return new EventMappers<TEvent, TProjection>(_mappers);
        }
    }
}