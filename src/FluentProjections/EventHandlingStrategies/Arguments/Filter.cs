using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Filter<TEvent>
    {
        private static readonly ILog<TEvent> Logger = LogProvider<TEvent>.GetLogger(typeof (Filters<TEvent>));

        private readonly Func<TEvent, object> _getValue;
        private readonly PropertyInfo _property;

        private Filter(PropertyInfo property, Func<TEvent, object> getValue)
        {
            _property = property;
            _getValue = getValue;
        }

        public FluentProjectionFilterValue GetValue(TEvent @event)
        {
            object value = _getValue(@event);

            Logger.DebugFormat("Filter {0} : {1}", _property.Name, value);

            return new FluentProjectionFilterValue(_property, value);
        }

        public static Filter<TEvent> Create<TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            var memberExpression = (MemberExpression) projectionProperty.Body;
            var property = (PropertyInfo) memberExpression.Member;
            return new Filter<TEvent>(property, e => getValue(e));
        }

        public static Filter<TEvent> Create<TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            TValue value)
        {
            var unaryExpression = projectionProperty.Body as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                var memberExpression = (MemberExpression) unaryExpression.Operand;
                var property = (PropertyInfo) memberExpression.Member;
                return new Filter<TEvent>(property, e => value);
            }
            else
            {
                var memberExpression = (MemberExpression) projectionProperty.Body;
                var property = (PropertyInfo) memberExpression.Member;
                return new Filter<TEvent>(property, e => value);
            }
        }
    }
}