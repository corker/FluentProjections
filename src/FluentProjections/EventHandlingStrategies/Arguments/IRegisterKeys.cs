namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public interface IRegisterKeys<TEvent, TProjection>: IRegisterMappers<TEvent, TProjection>
    {
        void Register(Key<TEvent, TProjection> key);
    }
}