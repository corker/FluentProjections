namespace FluentProjections.EventHandlingStrategies
{
    public interface IEventHandlingStrategy
    {
        void Handle(object @event, IFluentProjectionStore store);
    }

    public interface IEventHandlingStrategy<in TEvent>: IEventHandlingStrategy
    {
        void Handle(TEvent @event, IFluentProjectionStore store);
    }
}