namespace FluentProjections.EventHandlers.Arguments.Builders
{
    public interface IMapperArgumentsBuilder<TEvent, TProjection>
    {
        void AddMapper(EventMapper<TEvent, TProjection> mapper);
    }
}