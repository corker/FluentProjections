using System.Collections.Generic;

namespace FluentProjections.EventHandlers.Arguments
{
    public class ArgumentsBuilder<TEvent, TProjection> :
        IEventMapperBuilder<TEvent, TProjection>
    {
        private readonly List<ProjectionFilter<TEvent>> _filters;
        private readonly List<EventMapper<TEvent, TProjection>> _mappers;

        public ArgumentsBuilder()
        {
            _filters = new List<ProjectionFilter<TEvent>>();
            _mappers = new List<EventMapper<TEvent, TProjection>>();
        }

        public void AddFilter(ProjectionFilter<TEvent> filter)
        {
            _filters.Add(filter);
        }

        public void AddMapper(EventMapper<TEvent, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public ProjectionFilters<TEvent> BuildFilters()
        {
            return new ProjectionFilters<TEvent>(_filters);
        }

        public EventMappers<TEvent, TProjection> BuildMappers()
        {
            return new EventMappers<TEvent, TProjection>(_mappers);
        }
    }
}