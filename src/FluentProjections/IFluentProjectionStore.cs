using System.Collections.Generic;

namespace FluentProjections
{
    public interface IFluentProjectionStore
    {
        IEnumerable<TProjection> Read<TProjection>(FluentProjectionFilterValues values);
        void Update<TProjection>(TProjection projection);
        void Insert<TProjection>(TProjection projection);
    }
}