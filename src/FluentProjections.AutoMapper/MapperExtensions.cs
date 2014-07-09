using FluentProjections.EventHandlingStrategies.Arguments;
using AutoMapperMapper = AutoMapper.Mapper;

namespace FluentProjections.AutoMapper
{
    public static class MapperExtensions
    {
        /// <summary>
        ///     Map an event to a projection using AutoMapper
        /// </summary>
        /// <typeparam name="TEvent">An event type</typeparam>
        /// <typeparam name="TProjection">A projection type</typeparam>
        /// <param name="source">An argument builder that contains resulting mapper</param>
        /// <returns>An argument builder that contains resulting mapper</returns>
        public static IRegisterMappers<TEvent, TProjection> AutoMap<TEvent, TProjection>(
            this IRegisterMappers<TEvent, TProjection> source)
        {
            source.Do((e, p) => AutoMapperMapper.Map(e, p));
            return source;
        }
    }
}