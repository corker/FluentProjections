using System;
using System.Threading.Tasks;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;
using FluentProjections.Persistence;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections.Strategies
{
    public class AddNewProjectionStrategy<TMessage, TProjection> : IMessageHandlingStrategy<TMessage>
        where TProjection : class, new()
    {
        private static readonly ILog<TMessage, TProjection> Logger =
            LogProvider<TMessage, TProjection>.GetLogger(typeof(AddNewProjectionStrategy<TMessage, TProjection>));

        private readonly Mappers<TMessage, TProjection> _mappers;

        public AddNewProjectionStrategy(Mappers<TMessage, TProjection> mappers)
        {
            _mappers = mappers;
        }

        public virtual async Task HandleAsync(TMessage message, IProvideProjections store)
        {
            Logger.DebugFormat("Insert a projection because of a message: {0}", message);

            var projection = new TProjection();

            Map(message, projection);
            await InsertAsync(store, projection);
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
    }
}