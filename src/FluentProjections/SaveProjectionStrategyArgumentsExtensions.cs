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
            Mapper<TEvent, TProjection> mapper = GetMapper(projectionProperty, getValue);
            Filter<TEvent> filter = GetFilter(projectionProperty, getValue);
            source.AddKey(new Key<TEvent, TProjection>(filter, mapper));
            return source;
        }

        private static Mapper<TEvent, TProjection> GetMapper<TEvent, TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty, Func<TEvent, TValue> getValue)
        {
            var q = new Q<TEvent, TProjection>();
            q.Map(projectionProperty, getValue);
            return q.Mapper;
        }

        private static Filter<TEvent> GetFilter<TEvent, TProjection, TValue>(
            Expression<Func<TProjection, TValue>> projectionProperty, Func<TEvent, TValue> getValue)
        {
            var memberExpression = (MemberExpression) projectionProperty.Body;
            var property = (PropertyInfo) memberExpression.Member;
            return new Filter<TEvent>(property, e => getValue(e));
        }

        private class Q<TEvent, TProjection> : IMappersBuilder<TEvent, TProjection>
        {
            public Mapper<TEvent, TProjection> Mapper { get; private set; }

            public void AddMapper(Mapper<TEvent, TProjection> mapper)
            {
                Mapper = mapper;
            }
        }
    }
}