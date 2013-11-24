namespace FluentProjections
{
    public interface IFluentEventHandler<in TEvent>
    {
        void Handle(TEvent @event, IFluentProjectionStore store);
    }
}