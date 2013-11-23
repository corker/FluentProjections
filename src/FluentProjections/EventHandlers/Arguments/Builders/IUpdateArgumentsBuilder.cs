namespace FluentProjections.EventHandlers.Arguments.Builders
{
    public interface IUpdateArgumentsBuilder<TEvent, TProjection>: IMapperArgumentsBuilder<TEvent, TProjection>
    {
        void AddFilter(ProjectionFilter<TEvent> filter);
    }
}