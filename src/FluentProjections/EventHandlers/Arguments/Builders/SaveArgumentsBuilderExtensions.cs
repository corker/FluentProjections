using System;
using System.Linq.Expressions;

namespace FluentProjections.EventHandlers.Arguments.Builders
{
    public static class SaveArgumentsBuilderExtensions
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
        public static ISaveArgumentsBuilder<TEvent, TProjection> WithKey<TEvent, TProjection, TValue>(
            this ISaveArgumentsBuilder<TEvent, TProjection> source,
            Expression<Func<TProjection, TValue>> projectionProperty,
            Func<TEvent, TValue> getValue)
        {
            var q = new Q<TEvent, TProjection>();
            q.Map(projectionProperty, getValue);
            q.FilterBy(projectionProperty, getValue);
            source.AddKey(new ProjectionKey<TEvent, TProjection>(q.Filter, q.Mapper));
            return source;
        }

        private class Q<TEvent, TProjection> : 
            IMapperArgumentsBuilder<TEvent, TProjection>,
            IUpdateArgumentsBuilder<TEvent, TProjection>

        {
            public EventMapper<TEvent, TProjection> Mapper { get; private set; }
            public ProjectionFilter<TEvent> Filter { get; private set; }

            public void AddMapper(EventMapper<TEvent, TProjection> mapper)
            {
                Mapper = mapper;
            }

            public void AddFilter(ProjectionFilter<TEvent> filter)
            {
                Filter = filter;
            }
        }
    }
}