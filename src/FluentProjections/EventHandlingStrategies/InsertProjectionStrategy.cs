using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections.EventHandlingStrategies
{
    public class InsertProjectionStrategy<TEvent, TProjection> : IFluentEventHandlingStrategy<TEvent>
        where TProjection : class, new()
    {
        private readonly Mappers<TEvent, TProjection> _mappers;

        public InsertProjectionStrategy(Mappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
        {
            var projection = new TProjection();
            _mappers.Map(@event, projection);
            store.Insert(projection);
        }
    }
}