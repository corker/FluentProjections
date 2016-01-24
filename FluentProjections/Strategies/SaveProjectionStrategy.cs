using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;
using FluentProjections.Persistence;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class SaveProjectionStrategy<TMessage, TProjection> : IMessageHandlingStrategy<TMessage>
        where TProjection : class, new()
    {
        private static readonly ILog<TMessage, TProjection> Logger =
            LogProvider<TMessage, TProjection>.GetLogger(typeof (SaveProjectionStrategy<TMessage, TProjection>));

        private readonly Keys<TMessage, TProjection> _keys;
        private readonly Mappers<TMessage, TProjection> _mappers;

        public SaveProjectionStrategy(
            Keys<TMessage, TProjection> keys,
            Mappers<TMessage, TProjection> mappers)
        {
            _mappers = mappers;
            _keys = keys;
        }

        public virtual void Handle(TMessage message, IProvideProjections store)
        {
            Logger.DebugFormat("Save a projection because of a message: {0}", message);

            var filterValues = GetFilterValues(message);
            var projection = Read(store, filterValues);

            if (projection == null)
            {
                Logger.Debug("No projections found.");

                projection = new TProjection();
                MapKeys(message, projection);
                Map(message, projection);
                Insert(store, projection);
            }

            Logger.DebugFormat("A projection found: {0}", projection);

            Map(message, projection);
            Update(store, projection);
        }

        private IEnumerable<FilterValue> GetFilterValues(TMessage message)
        {
            Logger.Debug("Get filter values from a message.");
            try
            {
                return _keys.GetValues(message).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to get filter values.", e);
                throw;
            }
        }

        private static TProjection Read(IProvideProjections store, IEnumerable<FilterValue> filterValues)
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

        private static void Insert(IProvideProjections store, TProjection projection)
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

        private static void Update(IProvideProjections store, TProjection projection)
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

        private void Map(TMessage message, TProjection projection)
        {
            Logger.Debug("Map a message to a projection.");
            try
            {
                _mappers.Map(message, projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to map a message to a projection.", e);
                throw;
            }
        }

        private void MapKeys(TMessage message, TProjection projection)
        {
            Logger.Debug("Map keys to a projection.");
            try
            {
                _keys.Map(message, projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to map keys from a message to a projection.", e);
                throw;
            }
        }
    }
}