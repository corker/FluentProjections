namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public interface IMappersBuilder<TEvent, TProjection>
    {
        void AddMapper(Mapper<TEvent, TProjection> mapper);
    }
}