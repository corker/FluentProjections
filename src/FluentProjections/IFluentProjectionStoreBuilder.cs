namespace FluentProjections
{
    public interface IFluentProjectionStoreBuilder<TProjection>
    {
        IFluentProjectionStore<TProjection> Build();
    }
}