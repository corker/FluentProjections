namespace FluentProjections.EventHandlers.Arguments
{
    public interface IFiltersBuilder<TEvent, TProjection>: IMappersBuilder<TEvent, TProjection>
    {
        void AddFilter(Filter<TEvent> filter);
    }
}