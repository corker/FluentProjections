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
        public static AddNewProjectionStrategyArguments<TEvent, TProjection> AddNew<TEvent, TProjection>(
            this EventHandlingStrategyFactory<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new AddNewProjectionStrategyArguments<TEvent, TProjection>();
            source.SetFactoryMethod(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                return (IEventHandlingStrategy<TEvent>) new AddNewProjectionStrategy<TEvent, TProjection>(mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update all projections that match provided filters.
        /// </summary>
        public static UpdateProjectionStrategyArguments<TEvent, TProjection> Update<TEvent, TProjection>(
            this EventHandlingStrategyFactory<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new UpdateProjectionStrategyArguments<TEvent, TProjection>();
            source.SetFactoryMethod(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Filters<TEvent> filters = arguments.Filters;
                return (IEventHandlingStrategy<TEvent>) new UpdateProjectionStrategy<TEvent, TProjection>(filters, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update a projection that matches provided keys or insert a new projection when one doesn't exist.
        /// </summary>
        public static SaveProjectionStrategyArguments<TEvent, TProjection> Save<TEvent, TProjection>(
            this EventHandlingStrategyFactory<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new SaveProjectionStrategyArguments<TEvent, TProjection>();
            source.SetFactoryMethod(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Keys<TEvent, TProjection> keys = arguments.Keys;
                return (IEventHandlingStrategy<TEvent>) new SaveProjectionStrategy<TEvent, TProjection>(keys, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Translate an event into a series of other events.
        /// </summary>
        public static EventHandlingStrategyFactory<TR, TProjection> Translate<TEvent, TProjection, TR>(
            this EventHandlingStrategyFactory<TEvent, TProjection> source,
            Func<TEvent, IEnumerable<TR>> translate
            ) where TProjection : class, new()
        {
            var factory = new EventHandlingStrategyFactory<TR, TProjection>();
            source.SetFactoryMethod(() =>
            {
                IEventHandlingStrategy<TR> strategy = factory.Create();
                return (IEventHandlingStrategy<TEvent>) new TranslateStrategy<TEvent, TR>(translate, strategy);
            });
            return factory;
        }
    }
}