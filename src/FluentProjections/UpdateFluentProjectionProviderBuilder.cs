namespace FluentProjections
{
    public class UpdateFluentProjectionProviderBuilder<TProjection> : FluentProjectionProviderBuilder<TProjection>
    {
        public override FluentProjectionProvider<TProjection> Build()
        {
            return new UpdateFluentProjectionProvider<TProjection>();
        }
    }
}