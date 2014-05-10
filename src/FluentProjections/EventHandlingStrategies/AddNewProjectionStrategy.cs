using System;
using FluentProjections.EventHandlingStrategies.Arguments;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class AddNewProjectionStrategy<TEvent, TProjection> : EventHandlingStrategy<TEvent>
        where TProjection : class, new()
    {
        private static readonly ILog<TEvent, TProjection> Logger =
            LogProvider<TEvent, TProjection>.GetLogger(typeof (AddNewProjectionStrategy<TEvent, TProjection>));

        private readonly Mappers<TEvent, TProjection> _mappers;

        public AddNewProjectionStrategy(Mappers<TEvent, TProjection> mappers)
        {
            _mappers = mappers;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            Logger.DebugFormat("Insert a projection because of an event: {0}", @event);

            var projection = new TProjection();

            Map(@event, projection);
            Insert(store, projection);
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
    }
}