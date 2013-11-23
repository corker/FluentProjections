namespace FluentProjections.EventHandlers.Arguments
{
    public class ProjectionKey<TEvent, TProjection>
    {
        public ProjectionFilter<TEvent> Filter { get; private set; }
        public EventMapper<TEvent, TProjection> Mapper { get; private set; }

        public ProjectionKey(ProjectionFilter<TEvent> filter, EventMapper<TEvent, TProjection> mapper)
        {
            Filter = filter;
            Mapper = mapper;
        }
    }
}