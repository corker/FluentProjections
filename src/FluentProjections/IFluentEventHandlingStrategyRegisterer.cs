namespace FluentProjections
{
    public interface IFluentEventHandlingStrategyRegisterer
    {
        void Register<TEvent>(IFluentEventHandlingStrategy<TEvent> fluentEventHandlingStrategy);
    }
}