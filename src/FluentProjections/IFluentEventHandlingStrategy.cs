namespace FluentProjections
{
    public interface IFluentEventHandlingStrategy<in TEvent>
    {
        void Handle(TEvent @event, IFluentProjectionStore store);
    }
}