using System;
using System.Collections.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class TranslateStrategy<TEvent, TTranslatedEvent> : EventHandlingStrategy<TEvent>
    {
        private readonly Func<TEvent, IEnumerable<TTranslatedEvent>> _translate;
        private readonly IEventHandlingStrategy<TTranslatedEvent> _strategy;

        public TranslateStrategy(Func<TEvent, IEnumerable<TTranslatedEvent>> translate,
            IEventHandlingStrategy<TTranslatedEvent> strategy)
        {
            _translate = translate;
            _strategy = strategy;
        }

        public override void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<TTranslatedEvent> translatedEvents = _translate(@event);
            foreach (TTranslatedEvent translatedEvent  in translatedEvents)
            {
                _strategy.Handle(translatedEvent, store);
            }
        }
    }
}