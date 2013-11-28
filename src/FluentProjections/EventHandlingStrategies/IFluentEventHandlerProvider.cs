namespace FluentProjections.EventHandlingStrategies
{
    public interface IFluentEventHandlerProvider
    {
        void RegisterBy(IFluentEventHandlerRegisterer registerer);
    }
}