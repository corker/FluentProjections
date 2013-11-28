using System.Collections.Generic;

namespace FluentProjections
{
    public interface IFluentProjectionStore
    {
        IEnumerable<TProjection> Read<TProjection>(IEnumerable<FluentProjectionFilterValue> values) where TProjection: class;
        void Update<TProjection>(TProjection projection) where TProjection : class;
        void Insert<TProjection>(TProjection projection) where TProjection : class;
    }
}