using System.Collections.Generic;
using System.Linq;
using FluentProjections.EventHandlingStrategies;

namespace FluentProjections
{
    /// <summary>
    ///     A base class for a projection denormalizer.
    /// </summary>
    /// <typeparam name="TProjection">A type to project events on</typeparam>
    public abstract class FluentEventDenormalizer<TProjection> where TProjection : class, new()
    {
        private readonly List<IEventHandlingStrategyFactory> _factories;

        protected FluentEventDenormalizer()
        {
            _factories = new List<IEventHandlingStrategyFactory>();
        }

        /// <summary>
        ///     Register an event handler that can be configured with extensions.
        /// </summary>
        /// <typeparam name="TEvent">A type of an event</typeparam>
        protected EventHandlingStrategyFactory<TEvent, TProjection> When<TEvent>()
        {
            var factory = new EventHandlingStrategyFactory<TEvent, TProjection>();
            _factories.Add(factory);
            return factory;
        }

        /// <summary>
        ///     Handle event with all registered handlers.
        /// </summary>
        /// <param name="event">An event to be handled</param>
        /// <param name="store">A store to read and write projections</param>
        protected void Handle(object @event, IFluentProjectionStore store)
        {
            IEnumerable<IEventHandlingStrategy> strategies = _factories.Select(x => x.Create());

            foreach (IEventHandlingStrategy strategy in strategies)
            {
                strategy.Handle(@event, store);
            }
        }

        /// <summary>
        ///     Handle event of type <typeparam name="TEvent"></typeparam> with handlers registered for this type.
        /// </summary>
        /// <typeparam name="TEvent">A type of event</typeparam>
        /// <param name="event">An event to be handled</param>
        /// <param name="store">A store to read and write projections</param>
        protected void Handle<TEvent>(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<IEventHandlingStrategy<TEvent>> strategies = _factories
                .OfType<IEventHandlingStrategyFactory<TEvent>>()
                .Select(x => x.Create());

            foreach (var strategy in strategies)
            {
                strategy.Handle(@event, store);
            }
        }
    }
}