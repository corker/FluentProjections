namespace FluentProjections.EventHandlers.Arguments
{
    public interface IKeysBuilder<TEvent, TProjection>: IMappersBuilder<TEvent, TProjection>
    {
        void AddKey(Key<TEvent, TProjection> key);
    }
}