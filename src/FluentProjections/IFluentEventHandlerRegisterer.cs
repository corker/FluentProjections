namespace FluentProjections
{
    public interface IFluentEventHandlerRegisterer
    {
        void Register<TEvent, TProjection>(IFluentEventHandler<TEvent, TProjection> eventHandler);
    }
}