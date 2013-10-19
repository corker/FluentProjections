using System;

namespace FluentProjections
{
    public class FluentEventHandlerConfiguration<TEvent, TProjection> : IFluentEventHandlerConfiguration
        where TProjection : new()
    {
        private readonly FluentProjectionArgumentsBuilder<TEvent, TProjection> _argumentsBuilder;
        private FluentEventHandlerType _eventType;

        public FluentEventHandlerConfiguration()
        {
            _argumentsBuilder = new FluentProjectionArgumentsBuilder<TEvent, TProjection>();
        }

        public void RegisterBy(IFluentEventHandlerRegisterer registerer)
        {
            IFluentEventHandler<TEvent, TProjection> handler = Configure();
            registerer.Register(handler);
        }

        private IFluentEventHandler<TEvent, TProjection> Configure()
        {
            FluentProjectionArguments<TEvent, TProjection> arguments = _argumentsBuilder.Build();

            switch (_eventType)
            {
                case FluentEventHandlerType.Insert:
                    return new InsertFluentProjectionEventHandler<TEvent, TProjection>(arguments.Mappings);
                case FluentEventHandlerType.Update:
                    return new UpdateFluentProjectionEventHandler<TEvent, TProjection>(
                        arguments.Filters,
                        arguments.Mappings);
                default:
                    throw new NotImplementedException();
            }
        }

        public IFluentProjectionMappingsBuilder<TEvent, TProjection> AddNew()
        {
            _eventType = FluentEventHandlerType.Insert;
            return _argumentsBuilder;
        }

        public FluentProjectionArgumentsBuilder<TEvent, TProjection> Update()
        {
            _eventType = FluentEventHandlerType.Update;
            return _argumentsBuilder;
        }
    }
}