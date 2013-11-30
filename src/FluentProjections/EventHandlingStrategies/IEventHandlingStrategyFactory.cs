namespace FluentProjections.EventHandlingStrategies
{
    public interface IEventHandlingStrategyFactory
    {
        IEventHandlingStrategy Create();
    }

    public interface IEventHandlingStrategyFactory<in TEvent>
    {
        IEventHandlingStrategy<TEvent> Create();
    }
}