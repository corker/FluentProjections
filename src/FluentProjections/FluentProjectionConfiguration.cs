using System.Collections.Generic;

namespace FluentProjections
{
    public abstract class FluentProjectionConfiguration<TProjection> where TProjection : new()
    {
        private readonly List<IFluentEventHandlerConfiguration> _configurations =
            new List<IFluentEventHandlerConfiguration>();

        protected FluentEventHandlerConfiguration<TEvent, TProjection> ForEvent<TEvent>()
        {
            var configuration = new FluentEventHandlerConfiguration<TEvent, TProjection>();
            _configurations.Add(configuration);
            return configuration;
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            foreach (IFluentEventHandlerConfiguration configuration in _configurations)
            {
                configuration.RegisterBy(registerer);
            }
        }
    }
}