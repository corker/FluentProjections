using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;
using FluentProjections.Persistence;

namespace FluentProjections.Strategies.Arguments
{
    public class Filter<TMessage>
    {
        private static readonly ILog<TMessage> Logger = LogProvider<TMessage>.GetLogger(typeof (Filters<TMessage>));

        private readonly Func<TMessage, object> _getValue;
        private readonly PropertyInfo _property;

        private Filter(PropertyInfo property, Func<TMessage, object> getValue)
        {
            _property = property;
            _getValue = getValue;
        }

        public FilterValue GetValue(TMessage message)
        {
            object value = _getValue(message);

            Logger.DebugFormat("Filter {0} : {1}", _property.Name, value);

            return new FilterValue(_property, value);
        }

        public static Filter<TMessage> Create<TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TMessage, TValue> getValue)
        {
            var memberExpression = (MemberExpression) projectionProperty.Body;
            var property = (PropertyInfo) memberExpression.Member;
            return new Filter<TMessage>(property, e => getValue(e));
        }

        public static Filter<TMessage> Create<TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            TValue value)
        {
            var unaryExpression = projectionProperty.Body as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                var memberExpression = (MemberExpression) unaryExpression.Operand;
                var property = (PropertyInfo) memberExpression.Member;
                return new Filter<TMessage>(property, e => value);
            }
            else
            {
                var memberExpression = (MemberExpression) projectionProperty.Body;
                var property = (PropertyInfo) memberExpression.Member;
                return new Filter<TMessage>(property, e => value);
            }
        }
    }
}