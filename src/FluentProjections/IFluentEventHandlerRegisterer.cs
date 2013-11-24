using FluentProjections.EventHandlers;

namespace FluentProjections
{
    public interface IFluentEventHandlerRegisterer
    {
        void Register<TEvent>(IFluentEventHandler<TEvent> eventHandler);
    }
}