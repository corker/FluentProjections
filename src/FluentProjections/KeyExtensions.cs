using System;
using System.Linq.Expressions;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections
{
    public static class KeyExtensions
    {
        /// <summary>
        /// Update projection that matches a key or insert a new projection when no matching projection found.
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <typeparam name="TValue">A type of projection property</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <param name="projectionProperty">An expression that identifies a projection property</param>
        /// <param name="getValue">A function to extract a value from an event</param>
        /// <returns>An argument builder that contains resulting filter</returns>
        public static IKeysBuilder<TEvent, TProjection> WithKey<TEvent, TProjection, TValue>(
            this IKeysBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            var q = new Q<TEvent, TProjection>();
            q.Map(projectionProperty, getValue);
            q.FilterBy(projectionProperty, getValue);
            source.AddKey(new Key<TEvent, TProjection>(q.Filter, q.Mapper));
            return source;
        }

        private class Q<TEvent, TProjection> : IFiltersBuilder<TEvent, TProjection>

        {
            public Mapper<TEvent, TProjection> Mapper { get; private set; }
            public Filter<TEvent> Filter { get; private set; }

            public void AddMapper(Mapper<TEvent, TProjection> mapper)
            {
                Mapper = mapper;
            }

            public void AddFilter(Filter<TEvent> filter)
            {
                Filter = filter;
            }
        }
    }
}