using System;
using System.Collections.Generic;

namespace FluentProjections
{
    public class NewFluentProjectionProvider<TProjection> : FluentProjectionProvider<TProjection>
    {
        public override IEnumerable<TProjection> Read(FluentProjectionFilterValues filterValues)
        {
            throw new NotImplementedException();
        }

        public override void Save(TProjection projection)
        {
            throw new NotImplementedException();
        }
    }
}