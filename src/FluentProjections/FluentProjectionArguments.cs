namespace FluentProjections
{
    public class FluentProjectionArguments<TEvent, TProjection>
    {
        public FluentProjectionArguments(FluentProjectionFilters<TEvent> filters,
            FluentProjectionMappings<TEvent, TProjection> mappings)
        {
            Mappings = mappings;
            Filters = filters;
        }

        public FluentProjectionFilters<TEvent> Filters { get; private set; }
        public FluentProjectionMappings<TEvent, TProjection> Mappings { get; private set; }
    }
}