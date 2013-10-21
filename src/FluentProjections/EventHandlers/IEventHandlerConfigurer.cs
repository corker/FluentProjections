namespace FluentProjections.EventHandlers
{
    public interface IEventHandlerConfigurer
    {
        void RegisterBy(IFluentEventHandlerRegisterer registerer);
    }
}