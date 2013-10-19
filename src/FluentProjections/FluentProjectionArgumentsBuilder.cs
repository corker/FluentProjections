using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentProjections
{
    public class FluentProjectionArgumentsBuilder<TEvent, TProjection> :
        IFluentProjectionMappingsBuilder<TEvent, TProjection>
    {
        private readonly List<FluentProjectionFilter<TEvent>> _filters;
        private readonly List<FluentProjectionMapping<TEvent, TProjection>> _mappings;

        public FluentProjectionArgumentsBuilder()
        {
            _filters = new List<FluentProjectionFilter<TEvent>>();
            _mappings = new List<FluentProjectionMapping<TEvent, TProjection>>();
        }

        public IFluentProjectionMappingsBuilder<TEvent, TProjection> Map<TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            Action<TProjection, TValue> setter = GetSetter(projectionProperty);
            Action<TEvent, TProjection> mappingAction = (e, p) => setter(p, getValue(e));
            _mappings.Add(new FluentProjectionMapping<TEvent, TProjection>(mappingAction));
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

        public FluentProjectionArgumentsBuilder<TEvent, TProjection> FilterBy<TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, object> getValue)
        {
            var memberExpression = (MemberExpression) projectionProperty.Body;
            var property = (PropertyInfo) memberExpression.Member;

            _filters.Add(new FluentProjectionFilter<TEvent>(property, getValue));
            return this;
        }

        public FluentProjectionArguments<TEvent, TProjection> Build()
        {
            var filters = new FluentProjectionFilters<TEvent>(_filters);
            var mappings = new FluentProjectionMappings<TEvent, TProjection>(_mappings);
            return new FluentProjectionArguments<TEvent, TProjection>(filters, mappings);
        }
    }
}