namespace FluentProjections.EventHandlers.Arguments
{
    public interface IEventMapperBuilder<TEvent, TProjection>
    {
        void AddMapper(EventMapper<TEvent, TProjection> mapper);
    }
}