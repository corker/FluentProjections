using System.Collections.Generic;

namespace FluentProjections
{
    public interface IFluentProjectionStore
    {
        IEnumerable<TProjection> Read<TProjection>(IEnumerable<FluentProjectionFilterValue> values);
        void Update<TProjection>(TProjection projection);
        void Insert<TProjection>(TProjection projection);
    }
}