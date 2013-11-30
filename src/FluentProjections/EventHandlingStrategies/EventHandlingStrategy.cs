namespace FluentProjections.EventHandlingStrategies
{
    public abstract class EventHandlingStrategy<TEvent> : IEventHandlingStrategy<TEvent>
    {
        public abstract void Handle(TEvent @event, IFluentProjectionStore store);

        public void Handle(object @event, IFluentProjectionStore store)
        {
            if (@event is TEvent)
            {
                Handle((TEvent)@event, store);
            }
        }
    }
}