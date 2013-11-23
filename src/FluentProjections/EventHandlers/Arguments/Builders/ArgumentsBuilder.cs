using System.Collections.Generic;

namespace FluentProjections.EventHandlers.Arguments.Builders
{
    public class ArgumentsBuilder<TEvent, TProjection> :
        IMapperArgumentsBuilder<TEvent, TProjection>,
        IUpdateArgumentsBuilder<TEvent, TProjection>,
        ISaveArgumentsBuilder<TEvent, TProjection>
    {
        private readonly List<ProjectionFilter<TEvent>> _filters;
        private readonly List<EventMapper<TEvent, TProjection>> _mappers;
        private readonly List<ProjectionKey<TEvent, TProjection>> _keys;

        public ArgumentsBuilder()
        {
            _mappers = new List<EventMapper<TEvent, TProjection>>();
            _filters = new List<ProjectionFilter<TEvent>>();
            _keys = new List<ProjectionKey<TEvent, TProjection>>();
        }

        public void AddMapper(EventMapper<TEvent, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public void AddFilter(ProjectionFilter<TEvent> filter)
        {
            _filters.Add(filter);
        }

        public void AddKey(ProjectionKey<TEvent, TProjection> key)
        {
            _keys.Add(key);
        }

        public ProjectionFilters<TEvent> BuildFilters()
        {
            return new ProjectionFilters<TEvent>(_filters);
        }

        public EventMappers<TEvent, TProjection> BuildMappers()
        {
            return new EventMappers<TEvent, TProjection>(_mappers);
        }

        public ProjectionKeys<TEvent, TProjection> BuildKeys()
        {
            return new ProjectionKeys<TEvent, TProjection>(_keys);
        }
    }
}