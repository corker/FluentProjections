using System.Collections.Generic;

namespace FluentProjections
{
    public interface IFluentProjectionStore<TProjection>
    {
        IEnumerable<TProjection> Read(FluentProjectionFilterValues values);
        void Update(TProjection projection);
        void Insert(TProjection projection);
    }
}