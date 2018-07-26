namespace FluentProjections.Strategies.Arguments
{
    public interface IRegisterKeys<TMessage, TProjection>: IRegisterMappers<TMessage, TProjection>
    {
        void Register(Key<TMessage, TProjection> key);
    }
}