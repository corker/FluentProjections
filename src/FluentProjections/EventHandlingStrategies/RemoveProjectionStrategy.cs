using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.EventHandlingStrategies.Arguments;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class RemoveProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class
    {
        private static readonly ILog<TEvent, TProjection> Logger =
            LogProvider<TEvent, TProjection>.GetLogger(typeof (RemoveProjectionStrategy<TEvent, TProjection>));

        private readonly Filters<TEvent> _filters;

        public RemoveProjectionStrategy(Filters<TEvent> filters)
        {
            _filters = filters;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            Logger.DebugFormat("Remove projections because of an event: {0}", @event);
            IEnumerable<FluentProjectionFilterValue> filterValues = GetFilterValues(@event);
            Remove(store, filterValues);
        }

        private IEnumerable<FluentProjectionFilterValue> GetFilterValues(TEvent @event)
        {
            Logger.Debug("Get filter values from an event.");
            try
            {
                return _filters.GetValues(@event).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to get filter values.", e);
                throw;
            }
        }

        private static void Remove(IFluentProjectionStore store, IEnumerable<FluentProjectionFilterValue> filterValues)
        {
            Logger.Debug("Remove projections.");
            try
            {
                store.Remove<TProjection>(filterValues);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to remove projections.", e);
                throw;
            }
        }
    }
}