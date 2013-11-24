using System.Collections.Generic;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class UpdateProjectionEventHandlerArguments<TEvent, TProjection> : IFiltersBuilder<TEvent, TProjection>
    {
        private readonly List<Filter<TEvent>> _filters;
        private readonly List<Mapper<TEvent, TProjection>> _mappers;

        public UpdateProjectionEventHandlerArguments()
        {
            _mappers = new List<Mapper<TEvent, TProjection>>();
            _filters = new List<Filter<TEvent>>();
        }

        public void AddMapper(Mapper<TEvent, TProjection> mapper)
        {
            _mappers.Add(mapper);
        }

        public void AddFilter(Filter<TEvent> filter)
        {
            _filters.Add(filter);
        }

        public Filters<TEvent> Filters
        {
            get { return new Filters<TEvent>(_filters); }
        }

        public Mappers<TEvent, TProjection> Mappers
        {
            get { return new Mappers<TEvent, TProjection>(_mappers); }
        }
    }
}