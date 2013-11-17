using System;
using System.Collections.Generic;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections.EventHandlers
{
    public class EventHandlerConfigurer<TEvent, TProjection> : IEventHandlerConfigurer
        where TProjection : new()
    {
        private readonly ArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private Func<IFluentEventHandler<TEvent, TProjection>> _configure;

        public EventHandlerConfigurer()
        {
            _argumentsBuilder = new ArgumentsBuilder<TEvent, TProjection>();
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            IFluentEventHandler<TEvent, TProjection> handler = _configure();
            registerer.Register(handler);
        }

        public IEventMapperBuilder<TEvent, TProjection> AddNew()
        {
            _configure = ConfigureInsertEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent, TProjection> ConfigureInsertEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            return new InsertFluentProjectionEventHandler<TEvent, TProjection>(mappers);
        }

        public ArgumentsBuilder<TEvent, TProjection> Update()
        {
            _configure = ConfigureUpdateEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent, TProjection> ConfigureUpdateEventHandler()
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

        private static IFluentEventHandler<TEvent, TProjection> ConfigureTranslateEventHandler<TR>(
            Func<TEvent, IEnumerable<TR>> translate,
            EventHandlerConfigurer<TR, TProjection> configurer)
        {
            IFluentEventHandler<TR, TProjection> translatedEventHandler = configurer._configure();
            return new TranslateFluentProjectionEventHandler<TEvent, TProjection, TR>(translate, translatedEventHandler);
        }
    }
}