using System.Reflection;

namespace FluentProjections
{
    public class FluentProjectionFilterValue
    {
        public FluentProjectionFilterValue(PropertyInfo property, object value)
        {
            Property = property;
            Value = value;
        }

        public PropertyInfo Property { get; private set; }
        public object Value { get; private set; }
    }
}