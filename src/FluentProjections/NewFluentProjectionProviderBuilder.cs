namespace FluentProjections
{
    public class NewFluentProjectionProviderBuilder<TProjection> : FluentProjectionProviderBuilder<TProjection>
    {
        public override FluentProjectionProvider<TProjection> Build()
        {
            return new NewFluentProjectionProvider<TProjection>();
        }
    }
}