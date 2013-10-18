using System.Collections.Generic;

namespace FluentProjections
{
    public class FluentEventHandler<TEvent, TProjection> : IFluentEventHandler<TEvent>
    {
        private readonly FluentProjectionArguments<TEvent, TProjection> _arguments;
        private readonly FluentProjectionProvider<TProjection> _provider;

        public FluentEventHandler(FluentProjectionProvider<TProjection> provider,
            FluentProjectionArguments<TEvent, TProjection> arguments)
        {
            _provider = provider;
            _arguments = arguments;
        }

        public void Handle(TEvent @event)
        {
            FluentProjectionFilterValues filterValues = _arguments.Filter.GetValues(@event);
            IEnumerable<TProjection> projections = _provider.Read(filterValues);
            foreach (TProjection projection in projections)
            {
                _arguments.Mappings.Apply(@event, projection);
                _provider.Save(projection);
            }
        }
    }
}