using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentProjections
{
    public static class ReflectionHelpers
    {
        public static Action<TProjection, TValue> CreateBinaryOperation<TProjection, TValue>(
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
                        Expression.MakeBinary(type, Expression.Call(parameterProjection, getMethod),
                            parameterValue)),
                    parameterProjection,
                    parameterValue
                    );

            return lambda.Compile();
        }

        public static PropertyInfo GetEventPropertyInfo<TEvent, TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty)
        {
            PropertyInfo propertyInfo = typeof (TEvent).GetProperty(GetPropertyInfo(projectionProperty).Name);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException("projectionProperty", "No associated event property found.");
            }
            return propertyInfo;
        }

        private static PropertyInfo GetPropertyInfo<TProjection, TValue>(
            Expression<Func<TProjection, TValue>> expression)
        {
            return (PropertyInfo) ((MemberExpression) expression.Body).Member;
        }

        public static TValue GetPropertyValue<TEvent, TValue>(TEvent @event, PropertyInfo propertyInfo)
        {
            return (TValue) propertyInfo.GetValue(@event, new object[0]);
        }

        public static Action<TProjection, TValue> CreateSetOperation<TProjection, TValue>(
            Expression<Func<TProjection, TValue>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                var property = (PropertyInfo)((MemberExpression)unaryExpression.Operand).Member;
                MethodInfo setMethod = property.GetSetMethod();

                ParameterExpression parameterProjection = Expression.Parameter(typeof (TProjection), "projection");
                ParameterExpression parameterValue = Expression.Parameter(typeof (TValue), "value");

                Expression<Action<TProjection, TValue>> lambda =
                    Expression.Lambda<Action<TProjection, TValue>>(
                        Expression.Call(parameterProjection, setMethod, Expression.Convert(parameterValue, property.PropertyType)),
                        parameterProjection,
                        parameterValue
                        );
                return lambda.Compile();
            }
            else
            {
                PropertyInfo property = GetPropertyInfo(expression);
                MethodInfo setMethod = property.GetSetMethod();

                ParameterExpression parameterProjection = Expression.Parameter(typeof(TProjection), "projection");
                ParameterExpression parameterValue = Expression.Parameter(typeof(TValue), "value");

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
}