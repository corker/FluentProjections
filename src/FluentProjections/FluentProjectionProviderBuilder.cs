namespace FluentProjections
{
    public abstract class FluentProjectionProviderBuilder<TProjection>
    {
        public abstract FluentProjectionProvider<TProjection> Build();
    }
}