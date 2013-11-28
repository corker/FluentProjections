using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies;

namespace FluentProjections
{
    public abstract class FluentProjectionConfiguration<TProjection> where TProjection : class, new()
    {
        private readonly List<IFluentEventHandlerProvider> _configurers;

        protected FluentProjectionConfiguration()
        {
            _configurers = new List<IFluentEventHandlerProvider>();
        }

        protected FluentEventHandlerProvider<TEvent, TProjection> ForEvent<TEvent>()
        {
            var configuration = new FluentEventHandlerProvider<TEvent, TProjection>();
            _configurers.Add(configuration);
            return configuration;
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            foreach (IFluentEventHandlerProvider configuration in _configurers)
            {
                configuration.RegisterBy(registerer);
            }
        }
    }
}