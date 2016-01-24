namespace FluentProjections.Strategies.Arguments
{
    public interface IRegisterFilters<TMessage, TProjection>
    {
        void Register(Filter<TMessage> filter);
    }
}