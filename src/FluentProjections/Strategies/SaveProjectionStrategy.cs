using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            LogProvider<TMessage, TProjection>.GetLogger(typeof(SaveProjectionStrategy<TMessage, TProjection>));

        private readonly Keys<TMessage, TProjection> _keys;
        private readonly Mappers<TMessage, TProjection> _mappers;

        public SaveProjectionStrategy(
            Keys<TMessage, TProjection> keys,
            Mappers<TMessage, TProjection> mappers)
        {
            _mappers = mappers;
            _keys = keys;
        }

        public virtual async Task HandleAsync(TMessage message, IProvideProjections store)
        {
            Logger.DebugFormat("Save a projection because of a message: {0}", message);

            var filterValues = GetFilterValues(message);
            var projection = await ReadAsync(store, filterValues);

            if (projection == null)
            {
                Logger.Debug("No projections found.");

                projection = new TProjection();
                MapKeys(message, projection);
                Map(message, projection);
                await InsertAsync(store, projection);
            }

            Logger.DebugFormat("A projection found: {0}", projection);

            Map(message, projection);
            await UpdateAsync(store, projection);
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

        private static async Task<TProjection> ReadAsync(IProvideProjections store,
            IEnumerable<FilterValue> filterValues)
        {
            Logger.Debug("Read a projection.");
            try
            {
                return (await store.ReadAsync<TProjection>(filterValues)).SingleOrDefault();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to read a projection.", e);
                throw;
            }
        }

        private static async Task InsertAsync(IProvideProjections store, TProjection projection)
        {
            Logger.DebugFormat("Insert a projection: {0}", projection);
            try
            {
                await store.InsertAsync(projection);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to insert a projection.", e);
                throw;
            }
        }

        private static async Task UpdateAsync(IProvideProjections store, TProjection projection)
        {
            Logger.DebugFormat("Update a projection: {0}", projection);
            try
            {
                await store.UpdateAsync(projection);
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