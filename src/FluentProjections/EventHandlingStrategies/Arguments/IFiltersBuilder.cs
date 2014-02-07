namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public interface IFiltersBuilder<TEvent, TProjection>
    {
        void AddFilter(Filter<TEvent> filter);
    }
}