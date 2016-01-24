using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;
using FluentProjections.Persistence;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class UpdateProjectionStrategy<TMessage, TProjection> : IMessageHandlingStrategy<TMessage>
        where TProjection : class
    {
        private static readonly ILog<TMessage, TProjection> Logger =
            LogProvider<TMessage, TProjection>.GetLogger(typeof (UpdateProjectionStrategy<TMessage, TProjection>));

        private readonly Filters<TMessage> _filters;
        private readonly Mappers<TMessage, TProjection> _mappers;

        public UpdateProjectionStrategy(Filters<TMessage> filters,
            Mappers<TMessage, TProjection> mappers)
        {
            _filters = filters;
            _mappers = mappers;
        }

        public virtual void Handle(TMessage message, IProvideProjections store)
        {
            Logger.DebugFormat("Update projection(s) because of a message: {0}", message);

            var filterValues = GetFilterValues(message);
            var projections = Read(store, filterValues);
            foreach (var projection in projections)
            {
                Logger.DebugFormat("A projection found: {0}", projection);

                Map(message, projection);
                Update(store, projection);
            }
        }

        private static IEnumerable<TProjection> Read(
            IProvideProjections store,
            IEnumerable<FilterValue> filterValues
            )
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

        private IEnumerable<FilterValue> GetFilterValues(TMessage message)
        {
            Logger.Debug("Get filter values from a message.");
            try
            {
                return _filters.GetValues(message).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to get filter values.", e);
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
    }
}