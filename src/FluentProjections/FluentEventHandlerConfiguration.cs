namespace FluentProjections
{
    public class FluentEventHandlerConfiguration<TEvent, TProjection> : IFluentEventHandlerConfiguration
    {
        private FluentProjectionArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private FluentProjectionProviderBuilder<TProjection> _providerBuilder;

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            FluentEventHandler<TEvent, TProjection> handler = Configure();
            registerer.Register(handler);
        }

        private FluentEventHandler<TEvent, TProjection> Configure()
        {
            FluentProjectionProvider<TProjection> provider = _providerBuilder.Build();
            FluentProjectionArguments<TEvent, TProjection> arguments = _argumentsBuilder.Build();
            return new FluentEventHandler<TEvent, TProjection>(provider, arguments);
        }

        public IFluentProjectionMappingsBuilder<TEvent, TProjection> AddNew()
        {
            _providerBuilder = new NewFluentProjectionProviderBuilder<TProjection>();
            _argumentsBuilder = new FluentProjectionArgumentsBuilder<TEvent, TProjection>();
            return _argumentsBuilder;
        }

        public FluentProjectionArgumentsBuilder<TEvent, TProjection> Update()
        {
            _providerBuilder = new UpdateFluentProjectionProviderBuilder<TProjection>();
            _argumentsBuilder = new FluentProjectionArgumentsBuilder<TEvent, TProjection>();
            return _argumentsBuilder;
        }
    }
}