using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentProjections.EventHandlers.Arguments
{
    public class ArgumentsBuilder<TEvent, TProjection> :
        IEventMapperBuilder<TEvent, TProjection>
    {
        private readonly List<FluentProjectionFilter<TEvent>> _filters;
        private readonly List<EventMapper<TEvent, TProjection>> _mappers;

        public ArgumentsBuilder()
        {
            _filters = new List<FluentProjectionFilter<TEvent>>();
            _mappers = new List<EventMapper<TEvent, TProjection>>();
        }

        public IEventMapperBuilder<TEvent, TProjection> Map<TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            Action<TProjection, TValue> setter = GetSetter(projectionProperty);
            Action<TEvent, TProjection> action = (e, p) => setter(p, getValue(e));
            _mappers.Add(new EventMapper<TEvent, TProjection>(action));
            return this;
        }

        private static Action<TProjection, TValue> GetSetter<TValue>(Expression<Func<TProjection, TValue>> expression)
        {
            var memberExpression = (MemberExpression) expression.Body;
            var property = (PropertyInfo) memberExpression.Member;
            MethodInfo setMethod = property.GetSetMethod();

            ParameterExpression parameterProjection = Expression.Parameter(typeof (TProjection), "projection");
            ParameterExpression parameterValue = Expression.Parameter(typeof (TValue), "value");

            Expression<Action<TProjection, TValue>> setterExpression =
                Expression.Lambda<Action<TProjection, TValue>>(
                    Expression.Call(parameterProjection, setMethod, parameterValue),
                    parameterProjection,
                    parameterValue
                    );

            return setterExpression.Compile();
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