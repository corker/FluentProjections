namespace FluentProjections
{
    public interface IFluentEventHandler<in TEvent, TProjection>
    {
        void Handle(TEvent @event, IFluentProjectionStore<TProjection> store);
    }
}