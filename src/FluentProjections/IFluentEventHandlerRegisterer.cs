namespace FluentProjections
{
    public interface IFluentEventHandlerRegisterer
    {
        void Register<TEvent>(IFluentEventHandlingStrategy<TEvent> fluentEventHandlingStrategy);
    }
}