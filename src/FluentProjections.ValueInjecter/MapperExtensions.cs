using FluentProjections.EventHandlingStrategies.Arguments;
using Omu.ValueInjecter;

namespace FluentProjections.ValueInjecter
{
    public static class MapperExtensions
    {
        /// <summary>
        ///     Map an event to a projection using ValueInjecter
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TEvent, TProjection> Inject<TEvent, TProjection>(
            this IRegisterMappers<TEvent, TProjection> source)
        {
            source.Do((e, p) => p.InjectFrom(e));
            return source;
        }
    }
}