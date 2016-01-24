using System.Reflection;

namespace FluentProjections.Persistence
{
    public class FilterValue
    {
        public FilterValue(PropertyInfo property, object value)
        {
            Property = property;
            Value = value;
        }

        public PropertyInfo Property { get; private set; }
        public object Value { get; private set; }
    }
}