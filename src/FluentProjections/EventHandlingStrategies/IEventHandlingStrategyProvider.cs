namespace FluentProjections.EventHandlingStrategies
{
    public interface IEventHandlingStrategyProvider
    {
        void RegisterBy(IFluentEventHandlingStrategyRegisterer registerer);
    }
}