using System;
using System.Collections.Generic;

namespace FluentProjections.EventHandlers
{
    public class TranslateFluentProjectionEventHandler<TEvent, TProjection, TTranslatedEvent> : IFluentEventHandler<TEvent, TProjection>
    {
        private readonly Func<TEvent, IEnumerable<TTranslatedEvent>> _translate;
        private readonly IFluentEventHandler<TTranslatedEvent, TProjection> _translatedEventHandler;

        public TranslateFluentProjectionEventHandler(Func<TEvent, IEnumerable<TTranslatedEvent>> translate,
            IFluentEventHandler<TTranslatedEvent, TProjection> translatedEventHandler)
        {
            _translate = translate;
            _translatedEventHandler = translatedEventHandler;
        }

        public void Handle(TEvent @event, IFluentProjectionStore<TProjection> store)
        {
            IEnumerable<TTranslatedEvent> translatedEvents = _translate(@event);
            foreach (TTranslatedEvent translatedEvent  in translatedEvents)
            {
                _translatedEventHandler.Handle(translatedEvent, store);
            }
        }
    }
}