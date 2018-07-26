namespace FluentProjections.Strategies.Arguments
{
    public class Key<TMessage, TProjection>
    {
        private Key(Filter<TMessage> filter, Mapper<TMessage, TProjection> mapper)
        {
            Filter = filter;
            Mapper = mapper;
        }

        public Filter<TMessage> Filter { get; private set; }
        public Mapper<TMessage, TProjection> Mapper { get; private set; }

        public static Key<TMessage, TProjection> Create(Filter<TMessage> filter, Mapper<TMessage, TProjection> mapper)
        {
            return new Key<TMessage, TProjection>(filter, mapper);
        }
    }
}