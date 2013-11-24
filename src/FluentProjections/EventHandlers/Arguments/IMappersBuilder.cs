namespace FluentProjections.EventHandlers.Arguments
{
    public interface IMappersBuilder<TEvent, TProjection>
    {
        void AddMapper(Mapper<TEvent, TProjection> mapper);
    }
}