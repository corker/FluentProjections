using System;
using System.Collections.Generic;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class EventHandlerConfigurer<TEvent, TProjection> : IEventHandlerConfigurer
        where TProjection : new()
    {
        private readonly ArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private Func<IFluentEventHandler<TEvent>> _configure;

        public EventHandlerConfigurer()
        {
            _argumentsBuilder = new ArgumentsBuilder<TEvent, TProjection>();
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            IFluentEventHandler<TEvent> handler = _configure();
            registerer.Register(handler);
        }

        public IEventMapperBuilder<TEvent, TProjection> AddNew()
        {
            _configure = ConfigureInsertEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent> ConfigureInsertEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            return new InsertFluentProjectionEventHandler<TEvent, TProjection>(mappers);
        }

        public ArgumentsBuilder<TEvent, TProjection> Update()
        {
            _configure = ConfigureUpdateEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent> ConfigureUpdateEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            FluentProjectionFilters<TEvent> filters = _argumentsBuilder.BuildFilters();
            return new UpdateFluentProjectionEventHandler<TEvent, TProjection>(filters, mappers);
        }

        public EventHandlerConfigurer<TR, TProjection> Translate<TR>(Func<TEvent, IEnumerable<TR>> translate)
        {
            var configurer = new EventHandlerConfigurer<TR, TProjection>();
            _configure = () => ConfigureTranslateEventHandler(translate, configurer);
            return configurer;
        }

        private static IFluentEventHandler<TEvent> ConfigureTranslateEventHandler<TTranslatedEvent>(
            Func<TEvent, IEnumerable<TTranslatedEvent>> translate,
            EventHandlerConfigurer<TTranslatedEvent, TProjection> configurer)
        {
            IFluentEventHandler<TTranslatedEvent> translatedEventHandler = configurer._configure();
            return new TranslateFluentProjectionEventHandler<TEvent, TTranslatedEvent>(translate, translatedEventHandler);
        }
    }
}