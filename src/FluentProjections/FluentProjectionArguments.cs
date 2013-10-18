namespace FluentProjections
{
    public class FluentProjectionArguments<TEvent, TProjection>
    {
        public FluentProjectionFilter<TEvent> Filter { get; set; }
        public FluentProjectionMappings<TEvent, TProjection> Mappings { get; set; }
    }
}