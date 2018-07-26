using System;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.Strategies.Arguments
{
    public class Mapper<TMessage, TProjection>
    {
        private static readonly ILog<TMessage, TProjection> Logger = LogProvider<TMessage, TProjection>.GetLogger(typeof (Mapper<TMessage, TProjection>));

        private readonly Action<TMessage, TProjection> _action;

        private Mapper(Action<TMessage, TProjection> action)
        {
            _action = action;
        }

        public void Apply(TMessage message, TProjection projection)
        {
            try
            {
                _action(message, projection);
            }
            catch (Exception e)
            {
                string msg = string.Format("Failed to map a message {0} on a projection {1}.", message, projection);
                Logger.ErrorException(msg, e);
                throw;
            }
        }

        public static Mapper<TMessage, TProjection> Create(Action<TMessage, TProjection> action)
        {
            return new Mapper<TMessage, TProjection>(action);
        }
    }
}