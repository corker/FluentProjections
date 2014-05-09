namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public interface IRegisterFilters<TEvent, TProjection>
    {
        void Register(Filter<TEvent> filter);
    }
}