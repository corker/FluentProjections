using System;

namespace FluentProjections.Persistence
{
    public interface IUnitOfWork
    {
        void Commit();

    }
}