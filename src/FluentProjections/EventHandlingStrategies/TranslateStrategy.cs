using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class TranslateStrategy<TEvent, TTranslatedEvent> : EventHandlingStrategy<TEvent>
    {
        private static readonly ILog<TEvent, TTranslatedEvent> Logger =
            LogProvider<TEvent, TTranslatedEvent>.GetLogger(typeof (TranslateStrategy<TEvent, TTranslatedEvent>));

        private readonly IEventHandlingStrategy<TTranslatedEvent> _strategy;
        private readonly Func<TEvent, IEnumerable<TTranslatedEvent>> _translate;

        public TranslateStrategy(Func<TEvent, IEnumerable<TTranslatedEvent>> translate,
            IEventHandlingStrategy<TTranslatedEvent> strategy)
        {
            _translate = translate;
            _strategy = strategy;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<TTranslatedEvent> translatedEvents = Translate(@event);
            foreach (TTranslatedEvent translatedEvent in translatedEvents)
            {
                Handle(translatedEvent, store);
            }
        }

        private IEnumerable<TTranslatedEvent> Translate(TEvent @event)
        {
            Logger.DebugFormat("Translate an event: {0}", @event);
            try
            {
                return _translate(@event).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to translate an event.", e);
                throw;
            }
        }

        private void Handle(TTranslatedEvent @event, IFluentProjectionStore store)
        {
            Logger.DebugFormat("Handle a translated event: {0}", @event);
            try
            {
                _strategy.Handle(@event, store);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to handle translated event.", e);
                throw;
            }
        }
    }
}