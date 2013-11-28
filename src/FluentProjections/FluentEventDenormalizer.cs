using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies;

namespace FluentProjections
{
    public abstract class FluentEventDenormalizer<TProjection> where TProjection : class, new()
    {
        private readonly List<IEventHandlingStrategyProvider> _providers;

        protected FluentEventDenormalizer()
        {
            _providers = new List<IEventHandlingStrategyProvider>();
        }

        protected EventHandlingStrategyProvider<TEvent, TProjection> ForEvent<TEvent>()
        {
            var configuration = new EventHandlingStrategyProvider<TEvent, TProjection>();
            _providers.Add(configuration);
            return configuration;
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            foreach (IEventHandlingStrategyProvider configuration in _providers)
            {
                configuration.RegisterBy(registerer);
            }
        }
    }
}