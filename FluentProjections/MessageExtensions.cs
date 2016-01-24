using System;
using System.Collections.Generic;
using FluentProjections.Strategies;
using FluentProjections.Strategies.Arguments;

namespace FluentProjections
{
    public static class MessageExtensions
    {
        /// <summary>
        ///     Insert a new projection.
        /// </summary>
        public static AddNewProjectionStrategyArguments<TMessage, TProjection> AddNew<TMessage, TProjection>(
            this IMessageHandlingStrategyConfiguration<TMessage, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new AddNewProjectionStrategyArguments<TMessage, TProjection>();
            ((IContainMessageHandlingStrategyFactory<TMessage, TProjection>)source).SetFactory(() =>
            {
                Mappers<TMessage, TProjection> mappers = arguments.Mappers;
                return new AddNewProjectionStrategy<TMessage, TProjection>(mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update all projections that match provided filters.
        /// </summary>
        public static UpdateProjectionStrategyArguments<TMessage, TProjection> Update<TMessage, TProjection>(
            this IMessageHandlingStrategyConfiguration<TMessage, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new UpdateProjectionStrategyArguments<TMessage, TProjection>();
            ((IContainMessageHandlingStrategyFactory<TMessage, TProjection>)source).SetFactory(() =>
            {
                Mappers<TMessage, TProjection> mappers = arguments.Mappers;
                Filters<TMessage> filters = arguments.Filters;
                return new UpdateProjectionStrategy<TMessage, TProjection>(filters, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Update a projection that matches provided keys or insert a new projection when one doesn't exist.
        /// </summary>
        public static SaveProjectionStrategyArguments<TMessage, TProjection> Save<TMessage, TProjection>(
            this IMessageHandlingStrategyConfiguration<TMessage, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new SaveProjectionStrategyArguments<TMessage, TProjection>();
            ((IContainMessageHandlingStrategyFactory<TMessage, TProjection>)source).SetFactory(() =>
            {
                Mappers<TMessage, TProjection> mappers = arguments.Mappers;
                Keys<TMessage, TProjection> keys = arguments.Keys;
                return new SaveProjectionStrategy<TMessage, TProjection>(keys, mappers);
            });
            return arguments;
        }

        /// <summary>
        ///     Translate a message into a series of other messages.
        /// </summary>
        public static IMessageHandlingStrategyConfiguration<TR, TProjection> Translate<TMessage, TProjection, TR>(
            this IMessageHandlingStrategyConfiguration<TMessage, TProjection> source,
            Func<TMessage, IEnumerable<TR>> translate
            ) where TProjection : class, new()
        {
            var container = new MessageHandlingStrategyFactoryContainer<TR, TProjection>();
            ((IContainMessageHandlingStrategyFactory<TMessage, TProjection>)source).SetFactory(() =>
            {
                IMessageHandlingStrategy<TR> strategy = container.Create();
                return new TranslateStrategy<TMessage, TR>(translate, strategy);
            });
            return container;
        }

        /// <summary>
        ///     Remove projections.
        /// </summary>
        public static RemoveProjectionStrategyArguments<TMessage, TProjection> Remove<TMessage, TProjection>(
            this IMessageHandlingStrategyConfiguration<TMessage, TProjection> source
            ) where TProjection : class, new()
        {
            var arguments = new RemoveProjectionStrategyArguments<TMessage, TProjection>();
            ((IContainMessageHandlingStrategyFactory<TMessage, TProjection>)source).SetFactory(() =>
            {
                Filters<TMessage> filters = arguments.Filters;
                return new RemoveProjectionStrategy<TMessage, TProjection>(filters);
            });
            return arguments;
        }
    }
}