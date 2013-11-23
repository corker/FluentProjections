namespace FluentProjections.EventHandlers.Arguments.Builders
{
    public interface ISaveArgumentsBuilder<TEvent, TProjection> : IMapperArgumentsBuilder<TEvent, TProjection>
    {
        void AddKey(ProjectionKey<TEvent, TProjection> key);
    }
}