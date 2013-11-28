using System.Collections.Generic;
using System.Linq;
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

        protected void Handle<TEvent>(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<IFluentEventHandlingStrategy<TEvent>> strategies = _providers
                .OfType<EventHandlingStrategyProvider<TEvent, TProjection>>()
                .Select(x => x.Create());

            foreach (var strategy in strategies)
            {
                strategy.Handle(@event, store);
            }
        }

        public void RegisterStrategiesWith(IFluentEventHandlingStrategyRegisterer registerer)
        {
            foreach (IEventHandlingStrategyProvider provider in _providers)
            {
                provider.RegisterBy(registerer);
            }
        }
    }
}