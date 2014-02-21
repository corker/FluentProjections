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
            this IEventHandlingStrategyConfiguration<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new AddNewProjectionStrategyArguments<TEvent, TProjection>();
            ((IContainEventHandlingStrategyFactory<TEvent, TProjection>)source).SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                return new AddNewProjectionStrategy<TEvent, TProjection>(mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update all projections that match provided filters.
        /// </summary>
        public static UpdateProjectionStrategyArguments<TEvent, TProjection> Update<TEvent, TProjection>(
            this IEventHandlingStrategyConfiguration<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new UpdateProjectionStrategyArguments<TEvent, TProjection>();
            ((IContainEventHandlingStrategyFactory<TEvent, TProjection>)source).SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Filters<TEvent> filters = arguments.Filters;
                return new UpdateProjectionStrategy<TEvent, TProjection>(filters, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update a projection that matches provided keys or insert a new projection when one doesn't exist.
        /// </summary>
        public static SaveProjectionStrategyArguments<TEvent, TProjection> Save<TEvent, TProjection>(
            this IEventHandlingStrategyConfiguration<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new SaveProjectionStrategyArguments<TEvent, TProjection>();
            ((IContainEventHandlingStrategyFactory<TEvent, TProjection>)source).SetFactory(() =>
            {
                Mappers<TEvent, TProjection> mappers = arguments.Mappers;
                Keys<TEvent, TProjection> keys = arguments.Keys;
                return new SaveProjectionStrategy<TEvent, TProjection>(keys, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Translate an event into a series of other events.
        /// </summary>
        public static IEventHandlingStrategyConfiguration<TR, TProjection> Translate<TEvent, TProjection, TR>(
            this IEventHandlingStrategyConfiguration<TEvent, TProjection> source,
            Func<TEvent, IEnumerable<TR>> translate
            ) where TProjection : class, new()
        {
            var container = new EventHandlingStrategyFactoryContainer<TR, TProjection>();
            ((IContainEventHandlingStrategyFactory<TEvent, TProjection>)source).SetFactory(() =>
            {
                IEventHandlingStrategy<TR> strategy = container.Create();
                return new TranslateStrategy<TEvent, TR>(translate, strategy);
            });
            return container;
        }

        /// <summary>
        ///     Remove projections.
        /// </summary>
        public static RemoveProjectionStrategyArguments<TEvent, TProjection> Remove<TEvent, TProjection>(
            this IEventHandlingStrategyConfiguration<TEvent, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new RemoveProjectionStrategyArguments<TEvent, TProjection>();
            ((IContainEventHandlingStrategyFactory<TEvent, TProjection>)source).SetFactory(() =>
            {
                Filters<TEvent> filters = arguments.Filters;
                return new RemoveProjectionStrategy<TEvent, TProjection>(filters);
            });
            return arguments;
        }
    }
}