namespace FluentProjections
{
    public class InsertFluentProjectionEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent, TProjection>
        where TProjection : new()
    {
        private readonly FluentProjectionMappings<TEvent, TProjection> _mappings;

        public InsertFluentProjectionEventHandler(FluentProjectionMappings<TEvent, TProjection> mappings)
        {
            _mappings = mappings;
        }

        public void Handle(TEvent @event, IFluentProjectionStore<TProjection> store)
        {
            var projection = new TProjection();
            _mappings.Apply(@event, projection);
            store.Insert(projection);
        }
    }
}