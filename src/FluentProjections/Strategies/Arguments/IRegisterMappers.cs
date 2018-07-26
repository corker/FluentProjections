namespace FluentProjections.Strategies.Arguments
{
    public interface IRegisterMappers<TMessage, TProjection>
    {
        void Register(Mapper<TMessage, TProjection> mapper);
    }
}