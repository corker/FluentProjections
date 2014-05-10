using System;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies.Arguments
{
    public class Mapper<TEvent, TProjection>
    {
        private static readonly ILog<TEvent, TProjection> Logger = LogProvider<TEvent, TProjection>.GetLogger(typeof (Mapper<TEvent, TProjection>));

        private readonly Action<TEvent, TProjection> _action;

        private Mapper(Action<TEvent, TProjection> action)
        {
            _action = action;
        }

        public void Apply(TEvent @event, TProjection projection)
        {
            try
            {
                _action(@event, projection);
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to map an event {0} on a projection {1}.", @event, projection);
                Logger.ErrorException(message, e);
                throw;
            }
        }

        public static Mapper<TEvent, TProjection> Create(Action<TEvent, TProjection> action)
        {
            return new Mapper<TEvent, TProjection>(action);
        }
    }
}