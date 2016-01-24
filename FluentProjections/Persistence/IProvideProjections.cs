using System.Collections.Generic;

namespace FluentProjections.Persistence
{
    public interface IProvideProjections
    {
        IEnumerable<TProjection> Read<TProjection>(IEnumerable<FilterValue> values) where TProjection : class;
        void Update<TProjection>(TProjection projection) where TProjection : class;
        void Insert<TProjection>(TProjection projection) where TProjection : class;
        void Remove<TProjection>(IEnumerable<FilterValue> values) where TProjection : class;
    }
}