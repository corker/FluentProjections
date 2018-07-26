using System.Threading.Tasks;
using FluentProjections.Persistence;

namespace FluentProjections.Strategies
{
    public interface IMessageHandlingStrategy<in TMessage>
    {
        Task HandleAsync(TMessage message, IProvideProjections store);
    }
}