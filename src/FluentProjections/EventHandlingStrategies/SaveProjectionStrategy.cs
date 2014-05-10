using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.EventHandlingStrategies.Arguments;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class SaveProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class, new()
    {
        private static readonly ILog<TEvent, TProjection> Logger =
            LogProvider<TEvent, TProjection>.GetLogger(typeof (SaveProjectionStrategy<TEvent, TProjection>));

        private readonly Keys<TEvent, TProjection> _keys;
        private readonly Mappers<TEvent, TProjection> _mappers;

        public SaveProjectionStrategy(
            Keys<TEvent, TProjection> keys,
            Mappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
            _keys = keys;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            Logger.DebugFormat("Save a projection because of an event: {0}", @event);

            IEnumerable<FluentProjectionFilterValue> filterValues = GetFilterValues(@event);
            TProjection projection = Read(store, filterValues);

            if (projection == null)
            {
                Logger.Debug("No projections found.");

                projection = new TProjection();
                MapKeys(@event, projection);
                Map(@event, projection);
                Insert(store, projection);
            }
            else
            {
                Logger.DebugFormat("A projection found: {0}", projection);

                Map(@event, projection);
                Update(store, projection);
            }
        }

        private IEnumerable<FluentProjectionFilterValue> GetFilterValues(TEvent @event)
        {
            Logger.Debug("Get filter values from an event.");
            try
            {
                return _keys.GetValues(@event).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to get filter values.", e);
                throw;
            }
        }

        private static TProjection Read(IFluentProjectionStore store, IEnumerable<FluentProjectionFilterValue> filterValues)
        {
            Logger.Debug("Read a projection.");
            try
            {
                return store.Read<TProjection>(filterValues).SingleOrDefault();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to read a projection.", e);
                throw;
            }
        }

        private static void Insert(IFluentProjectionStore store, TProjection projection)
        {
            Logger.DebugFormat("Insert a projection: {0}", projection);
            try
            {
                store.Insert(projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to insert a projection.", e);
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

        private void MapKeys(TEvent @event, TProjection projection)
        {
            Logger.Debug("Map keys to a projection.");
            try
            {
                _keys.Map(@event, projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to map keys from an event to a projection.", e);
                throw;
            }
        }
    }
}