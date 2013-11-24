using System;
using System.Collections.Generic;
using FluentProjections.EventHandlers;
using FluentProjections.EventHandlers.Arguments;

namespace FluentProjections
{
    public static class EventExtensions
    {
        /// <summary>
        ///     Insert a new projection.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public static InsertProjectionEventHandlerArguments<TEvent, TProjection> Insert<TEvent, TProjection>(
            this FluentEventHandlerProvider<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new InsertProjectionEventHandlerArguments<TEvent, TProjection>();
            source.SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                return (IFluentEventHandler<TEvent>) new InsertProjectionEventHandler<TEvent, TProjection>(mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update all projections that match provided filters.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public static UpdateProjectionEventHandlerArguments<TEvent, TProjection> Update<TEvent, TProjection>(
            this FluentEventHandlerProvider<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new UpdateProjectionEventHandlerArguments<TEvent, TProjection>();
            source.SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Filters<TEvent> filters = arguments.Filters;
                return (IFluentEventHandler<TEvent>) new UpdateProjectionEventHandler<TEvent, TProjection>(filters, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update a projection that matches provided keys or insert a new projection when one doesn't exist.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public static SaveProjectionEventHandlerArguments<TEvent, TProjection> Save<TEvent, TProjection>(
            this FluentEventHandlerProvider<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new SaveProjectionEventHandlerArguments<TEvent, TProjection>();
            source.SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Keys<TEvent, TProjection> keys = arguments.Keys;
                return (IFluentEventHandler<TEvent>) new SaveProjectionEventHandler<TEvent, TProjection>(keys, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Translate an incoming event into a series of translated events that can be handled the same way as an original
        ///     event.
        /// </summary>
        /// <returns>A configurer for translated event.</returns>
        public static FluentEventHandlerProvider<TR, TProjection> Translate<TEvent, TProjection, TR>(
            this FluentEventHandlerProvider<TEvent, TProjection> source,
            Func<TEvent, IEnumerable<TR>> translate
            ) where TProjection : class, new()
        {
            var provider = new FluentEventHandlerProvider<TR, TProjection>();
            source.SetFactory(() =>
            {
                IFluentEventHandler<TR> translatedEventHandler = provider.Create();
                return (IFluentEventHandler<TEvent>) new TranslateEventHandler<TEvent, TR>(translate, translatedEventHandler);
            });
            return provider;
        }
    }
}