using System.Collections;
using System.Collections.Generic;

namespace FluentProjections
{
    public class FluentProjectionFilterValues: IEnumerable<FluentProjectionFilterValue>
    {
        private readonly IEnumerable<FluentProjectionFilterValue> _values;

        public FluentProjectionFilterValues(IEnumerable<FluentProjectionFilterValue> values)
        {
            _values = values;
        }

        public IEnumerator<FluentProjectionFilterValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}