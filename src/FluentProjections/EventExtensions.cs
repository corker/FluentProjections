using System;
using System.Collections.Generic;
using FluentProjections.EventHandlingStrategies;
using FluentProjections.EventHandlingStrategies.Arguments;

namespace FluentProjections
{
    public static class EventExtensions
    {
        /// <summary>
        ///     Insert a new projection.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public static InsertProjectionStrategyArguments<TEvent, TProjection> Insert<TEvent, TProjection>(
            this EventHandlingStrategyProvider<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new InsertProjectionStrategyArguments<TEvent, TProjection>();
            source.SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                return (IFluentEventHandlingStrategy<TEvent>) new InsertProjectionStrategy<TEvent, TProjection>(mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update all projections that match provided filters.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public static UpdateProjectionStrategyArguments<TEvent, TProjection> Update<TEvent, TProjection>(
            this EventHandlingStrategyProvider<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new UpdateProjectionStrategyArguments<TEvent, TProjection>();
            source.SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Filters<TEvent> filters = arguments.Filters;
                return (IFluentEventHandlingStrategy<TEvent>) new UpdateProjectionStrategy<TEvent, TProjection>(filters, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update a projection that matches provided keys or insert a new projection when one doesn't exist.
        /// </summary>
        /// <returns>An argument builder to configure a behavior.</returns>
        public static SaveProjectionStrategyArguments<TEvent, TProjection> Save<TEvent, TProjection>(
            this EventHandlingStrategyProvider<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new SaveProjectionStrategyArguments<TEvent, TProjection>();
            source.SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Keys<TEvent, TProjection> keys = arguments.Keys;
                return (IFluentEventHandlingStrategy<TEvent>) new SaveProjectionStrategy<TEvent, TProjection>(keys, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Translate an incoming event into a series of translated events that can be handled the same way as an original
        ///     event.
        /// </summary>
        /// <returns>A configurer for translated event.</returns>
        public static EventHandlingStrategyProvider<TR, TProjection> Translate<TEvent, TProjection, TR>(
            this EventHandlingStrategyProvider<TEvent, TProjection> source,
            Func<TEvent, IEnumerable<TR>> translate
            ) where TProjection : class, new()
        {
            var provider = new EventHandlingStrategyProvider<TR, TProjection>();
            source.SetFactory(() =>
            {
                IFluentEventHandlingStrategy<TR> strategy = provider.Create();
                return (IFluentEventHandlingStrategy<TEvent>) new TranslateStrategy<TEvent, TR>(translate, strategy);
            });
            return provider;
        }
    }
}