using System;
using System.Collections.Generic;

namespace FluentProjections.EventHandlers
{
    public class TranslateFluentProjectionEventHandler<TEvent, TTranslatedEvent> : IFluentEventHandler<TEvent>
    {
        private readonly Func<TEvent, IEnumerable<TTranslatedEvent>> _translate;
        private readonly IFluentEventHandler<TTranslatedEvent> _translatedEventHandler;

        public TranslateFluentProjectionEventHandler(Func<TEvent, IEnumerable<TTranslatedEvent>> translate,
            IFluentEventHandler<TTranslatedEvent> translatedEventHandler)
        {
            _translate = translate;
            _translatedEventHandler = translatedEventHandler;
        }

        public void Handle(TEvent @event, IFluentProjectionStore store)
        {
            IEnumerable<TTranslatedEvent> translatedEvents = _translate(@event);
            foreach (TTranslatedEvent translatedEvent  in translatedEvents)
            {
                _translatedEventHandler.Handle(translatedEvent, store);
            }
        }
    }
}