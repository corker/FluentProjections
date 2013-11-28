namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Key<TEvent, TProjection>
    {
        public Filter<TEvent> Filter { get; private set; }
        public Mapper<TEvent, TProjection> Mapper { get; private set; }

        public Key(Filter<TEvent> filter, Mapper<TEvent, TProjection> mapper)
        {
            Filter = filter;
            Mapper = mapper;
        }
    }
}