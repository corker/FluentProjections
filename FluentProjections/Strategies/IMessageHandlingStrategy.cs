using FluentProjections.Persistence;

namespace FluentProjections.Strategies
{
    public interface IMessageHandlingStrategy<in TMessage>
    {
        void Handle(TMessage message, IProvideProjections store);
    }
}