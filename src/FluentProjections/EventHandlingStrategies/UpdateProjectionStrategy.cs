using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.EventHandlingStrategies.Arguments;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class UpdateProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class
    {
        private static readonly ILog<TEvent, TProjection> Logger =
            LogProvider<TEvent, TProjection>.GetLogger(typeof (UpdateProjectionStrategy<TEvent, TProjection>));

        private readonly Filters<TEvent> _filters;
        private readonly Mappers<TEvent, TProjection> _mappers;

        public UpdateProjectionStrategy(Filters<TEvent> filters,
            Mappers<TEvent, TProjection> mappers)
        {
            _filters = filters;
            _mappers = mappers;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            Logger.DebugFormat("Update projection(s) because of an event: {0}", @event);

            IEnumerable<FluentProjectionFilterValue> filterValues = GetFilterValues(@event);
            IEnumerable<TProjection> projections = Read(store, filterValues);
            foreach (TProjection projection in projections)
            {
                Logger.DebugFormat("A projection found: {0}", projection);

                Map(@event, projection);
                Update(store, projection);
            }
        }

        private static IEnumerable<TProjection> Read(IFluentProjectionStore store, IEnumerable<FluentProjectionFilterValue> filterValues)
        {
            Logger.Debug("Read a projections.");
            try
            {
                return store.Read<TProjection>(filterValues);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to read projections.", e);
                throw;
            }
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

        private static void Update(IFluentProjectionStore store, TProjection projection)
        {
            Logger.DebugFormat("Update a projection: {0}", projection);
            try
            {
                store.Update(projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to update a projection.", e);
                throw;
            }
        }

        private void Map(TEvent @event, TProjection projection)
        {
            Logger.Debug("Map an event to a projection.");
            try
            {
                _mappers.Map(@event, projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to map an event to a projection.", e);
                throw;
            }
        }
    }
}