using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentProjections.Persistence
{
    public interface IProvideProjections
    {
        Task<IEnumerable<TProjection>> ReadAsync<TProjection>(IEnumerable<FilterValue> values)
            where TProjection : class;

        Task UpdateAsync<TProjection>(TProjection projection)
            where TProjection : class;

        Task InsertAsync<TProjection>(TProjection projection)
            where TProjection : class;

        Task RemoveAsync<TProjection>(IEnumerable<FilterValue> values)
            where TProjection : class;
    }
}