namespace FluentProjections
{
    public interface IFluentEventHandlerConfiguration
    {
        void RegisterBy(IFluentEventHandlerRegisterer registerer);
    }
}