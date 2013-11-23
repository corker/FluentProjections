using System;
using System.Collections.Generic;
using FluentProjections.EventHandlers.Arguments;
using FluentProjections.EventHandlers.Arguments.Builders;

namespace FluentProjections.EventHandlers
{
    public class EventHandlerConfigurer<TEvent, TProjection> : IEventHandlerConfigurer
        where TProjection : class, new()
    {
        private readonly ArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private Func<IFluentEventHandler<TEvent>> _configure;

        public EventHandlerConfigurer()
        {
            _argumentsBuilder = new ArgumentsBuilder<TEvent, TProjection>();
        }

        void IEventHandlerConfigurer.RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            IFluentEventHandler<TEvent> handler = _configure();
            registerer.Register(handler);
        }

        /// <summary>
        /// Insert a new projection.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public IMapperArgumentsBuilder<TEvent, TProjection> Insert()
        {
            _configure = ConfigureInsertEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent> ConfigureInsertEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            return new InsertFluentProjectionEventHandler<TEvent, TProjection>(mappers);
        }

        /// <summary>
        /// Update all projections that match provided filters.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public ArgumentsBuilder<TEvent, TProjection> Update()
        {
            _configure = ConfigureUpdateEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent> ConfigureUpdateEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            ProjectionFilters<TEvent> filters = _argumentsBuilder.BuildFilters();
            return new UpdateFluentProjectionEventHandler<TEvent, TProjection>(filters, mappers);
        }

        /// <summary>
        /// Update a projection that matches provided keys or insert a new projection when doesn't exist.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public ISaveArgumentsBuilder<TEvent, TProjection> Save()
        {
            _configure = ConfigureSaveEventHandler;
            return _argumentsBuilder;
        }

        private IFluentEventHandler<TEvent> ConfigureSaveEventHandler()
        {
            EventMappers<TEvent, TProjection> mappers = _argumentsBuilder.BuildMappers();
            ProjectionKeys<TEvent, TProjection> keys = _argumentsBuilder.BuildKeys();
            return new SaveFluentProjectionEventHandler<TEvent, TProjection>(keys, mappers);
        }

        /// <summary>
        /// Translate an incoming event into a series of translated events that can be handled the same way as an original event.
        /// </summary>
        /// <returns>A configurer for translated event.</returns>
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