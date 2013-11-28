namespace FluentProjections.EventHandlingStrategies
{
    public interface IEventHandlingStrategyProvider
    {
        void RegisterBy(IFluentEventHandlerRegisterer registerer);
    }
}