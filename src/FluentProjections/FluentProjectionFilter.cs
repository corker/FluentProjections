namespace FluentProjections
{
    public class FluentProjectionFilter<TEvent>
    {
        public FluentProjectionFilterValues GetValues(TEvent @event)
        {
            return new FluentProjectionFilterValues();
        }
    }
}