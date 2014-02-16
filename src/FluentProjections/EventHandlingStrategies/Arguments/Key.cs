namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Key<TEvent, TProjection>
    {
        private Key(Filter<TEvent> filter, Mapper<TEvent, TProjection> mapper)
        {
            Filter = filter;
            Mapper = mapper;
        }

        public Filter<TEvent> Filter { get; private set; }
        public Mapper<TEvent, TProjection> Mapper { get; private set; }

        public static Key<TEvent, TProjection> Create(Filter<TEvent> filter, Mapper<TEvent, TProjection> mapper)
        {
            return new Key<TEvent, TProjection>(filter, mapper);
        }
    }
}