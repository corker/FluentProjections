using System.Collections.Generic;

namespace FluentProjections
{
    public abstract class FluentProjectionProvider<TProjection>
    {
        public abstract IEnumerable<TProjection> Read(FluentProjectionFilterValues filterValues);
        public abstract void Save(TProjection projection);
    }
}