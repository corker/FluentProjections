namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public interface IRegisterMappers<TEvent, TProjection>
    {
        void Register(Mapper<TEvent, TProjection> mapper);
    }
}