using System.Collections.Generic;
using FluentProjections.EventHandlers;

namespace FluentProjections
{
    public abstract class FluentProjectionConfiguration<TProjection> where TProjection : new()
    {
        private readonly List<IEventHandlerConfigurer> _configurers;

        protected FluentProjectionConfiguration()
        {
            _configurers = new List<IEventHandlerConfigurer>();
        }

        protected EventHandlerConfigurer<TEvent, TProjection> ForEvent<TEvent>()
        {
            var configuration = new EventHandlerConfigurer<TEvent, TProjection>();
            _configurers.Add(configuration);
            return configuration;
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            foreach (IEventHandlerConfigurer configuration in _configurers)
            {
                configuration.RegisterBy(registerer);
            }
        }
    }
}