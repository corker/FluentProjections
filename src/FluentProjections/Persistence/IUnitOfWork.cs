using System.Threading.Tasks;

namespace FluentProjections.Persistence
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}