using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.EventHandlingStrategies;

namespace FluentProjections
{
    /// <summary>
    ///     A base class for your projection denormalizers.
    /// </summary>
    /// <typeparam name="TProjection">A type to project events on</typeparam>
    public abstract class FluentEventDenormalizer<TProjection> where TProjection : class, new()
    {
        private readonly Router _router;

        protected FluentEventDenormalizer()
        {
            _router = new Router();
        }

        /// <summary>
        ///     Register an event handler that can be configured with extensions.
        /// </summary>
        /// <typeparam name="TEvent">A type of an event</typeparam>
        protected void On<TEvent>(Action<EventHandlingStrategyFactoryContainer<TEvent, TProjection>> configurer)
        {
            _router.Add(configurer);
        }

        /// <summary>
        ///     Handle event by all registered handlers.
        /// </summary>
        /// <param name="event">An event to be handled</param>
        /// <param name="store">A store to read and write projections</param>
        protected void Handle(object @event, IFluentProjectionStore store)
        {
            _router.Route(@event, store);
        }

        private class Router
        {
            private readonly List<IHandleEvents> _handlers = new List<IHandleEvents>();

            public void Add<TEvent>(Action<EventHandlingStrategyFactoryContainer<TEvent, TProjection>> configurer)
            {
                _handlers.Add(new EventHandler<TEvent>(configurer));
            }

            public void Route(object @event, IFluentProjectionStore store)
            {
                IEnumerable<IHandleEvents> handlers = GetHandlers(@event);
                foreach (IHandleEvents handler in handlers)
                {
                    handler.Handle(@event, store);
                }
            }

            private IEnumerable<IHandleEvents> GetHandlers(object @event)
            {
                Type eventType = @event.GetType();
                return _handlers.Where(x => x.EventType == eventType);
            }

            private class EventHandler<TEvent> : IHandleEvents
            {
                private readonly Action<EventHandlingStrategyFactoryContainer<TEvent, TProjection>> _configurer;
                private EventHandlingStrategyFactoryContainer<TEvent, TProjection> _factoryContainer;
                private IEventHandlingStrategy<TEvent> _strategy;

                public EventHandler(Action<EventHandlingStrategyFactoryContainer<TEvent, TProjection>> configurer)
                {
                    EventType = typeof (TEvent);
                    _configurer = configurer;
                }

                public Type EventType { get; private set; }

                public void Handle(object @event, IFluentProjectionStore store)
                {
                    EnsureStrategy();
                    _strategy.Handle(@event, store);
                }

                private void EnsureStrategy()
                {
                    EnsureStrategyFactory();
                    _strategy = _strategy ?? _factoryContainer.Create();
                }

                private void EnsureStrategyFactory()
                {
                    if (_factoryContainer == null)
                    {
                        _factoryContainer = new EventHandlingStrategyFactoryContainer<TEvent, TProjection>();
                        _configurer(_factoryContainer);
                    }
                }
            }

            private interface IHandleEvents
            {
                Type EventType { get; }
                void Handle(object @event, IFluentProjectionStore store);
            }
        }
    }
}