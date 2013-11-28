using System;
using System.Collections.Generic;

namespace FluentProjections.EventHandlingStrategies
{
    public class TranslateStrategy<TEvent, TTranslatedEvent> : IFluentEventHandlingStrategy<TEvent>
    {
        private readonly Func<TEvent, IEnumerable<TTranslatedEvent>> _translate;
        private readonly IFluentEventHandlingStrategy<TTranslatedEvent> _strategy;

        public TranslateStrategy(Func<TEvent, IEnumerable<TTranslatedEvent>> translate,
            IFluentEventHandlingStrategy<TTranslatedEvent> strategy)
        {
            _translate = translate;
            _strategy = strategy;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<TTranslatedEvent> translatedEvents = _translate(@event);
            foreach (TTranslatedEvent translatedEvent  in translatedEvents)
            {
                _strategy.Handle(translatedEvent, store);
            }
        }
    }
}