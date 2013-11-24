namespace FluentProjections.EventHandlers
{
    public interface IFluentEventHandlerProvider
    {
        void RegisterBy(IFluentEventHandlerRegisterer registerer);
    }
}