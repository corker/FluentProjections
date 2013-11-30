fluent-projections
==================

Fluent Projections provides a configurable hub that can handle events and map them to database projection tables (read models).

Why?
====

When CQRS and Event Sourcing come in, you have to deal with projection tables or read models. Projection contains data prepared for a specific view. To process events we use so called denormalizers. Denormalizer is a hub that handle domain evens and map them to a projection. To define a mapping between events and a projection I have to write a very similar code.

I think it's a kind of waste of time to do it again and again.

What?
====

So, I noticed some patterns in this process:
* Direct mapping to a projection property with another or even the same name;
* All kind of aggregations;
* All kind of lookups.

I'm a big fan of configuring and I want to share with you the idea of configurable projection denormalizers that I call Fluent Projections.

How?
====

There is not much you can do right now - only to map properties directly.

There is no concrete persistence implementations, so stay tuned!

To be able to use FluentProjections you need to do the following steps.

1. Install a nuget package
--

```
Install-Package FluentProjections
```

2. Implement your projection configurations
--

Here is an example:

```
public class SeatProjectionDenormalizer : FluentEventDenormalizer<SeatProjection>
{
    private readonly IFluentProjectionStore _store;
    
    public SeatProjectionDenormalizer(IFluentProjectionStore store)
    {
        _store = store;

        ForEvent<ConcertCreated>()
            .Translate(concert => new[] // translate an event into a series of objects (optional)
            {
                new DefineSeat { Id = concert.Seats.First().SeatId, ... },
                new DefineSeat { Id = concert.Seats.Last().SeatId, ... }
            })
            .Insert() // insert a projection with data mapped from a translated object into a store
                .Map(projection => projection.Id)
                .Map(projection => projection.Location, seat.SeatLocation);
            
        ForEvent<SeatLocationCorrected>()
            .Update() // update all projections that matches provided filter(s) in a store
                .FilterBy(projection => projection.Id, @event => @event.Id)
                .Map(projection => projection.Location);
    }

    /// <summary>
    ///     This is a single handler for all registered events
    /// </summary>
    public void Handle(object @event)
    {
        Handle(@event, _store);
    }
}

public class MonthStatisticsDenormalizer : FluentEventDenormalizer<MonthStatistics>
{
    private readonly IFluentProjectionStore _store;

    public MonthStatisticsDenormalizer(IFluentProjectionStore store)
    {
        _store = store;

        ForEvent<ConcertCreated>()
            .Save() // update a projection that matches provided key(s) in a store or create a new one
                .Key(p => p.Year, e => e.Date.Year)
                .Key(p => p.Month, e => e.Date.Month)
                .Increment(p => p.Concerts);
    }

    /// <summary>
    ///     This is a handler for ConcertCreated event
    /// </summary>
    public void Handle(ConcertCreated @event)
    {
        Handle(@event, _store);
    }
}
```

3. Implement IFluentProjectionStore<TProjection>
--

IFluentProjectionStore<TProjection> is a persistence provider for your projections. It should be able to read, insert and update projections.

Later I'm going to provide implementation for different databases, but right now this is a manual work.

On a project website you can find a test project where more examples can be observed.

Happy coding!

What’s new?
-----------

**Nov 24th, 2013**      
- A codebase reorganized the way you need to reference the only root namespace.
- Save method introduced - see configuration examples above.
- AddNew method renamed to Insert so as it's more explicit.
- All methods like Insert, Update, Save, Translate are implemented as extensions - it's possible to have custom methods now.
- All methods like Map, FilterBy, Key are implemented as extensions - it's possible to have custom methods now.
- Class constraint added to TProjection in IFluentProjectionStore.

**Nov 17th, 2013**      
- A single event can be translated into a series of objects. Translated objects can be handled the same way as other events.

Who am I?
--
My name is Michael Borisov. I'm interested in CQRS, event sourcing and service oriented architecture.

If you have any comments please feel free to contact me on [Twitter](https://twitter.com/fkem) or [LinkedIn](https://www.linkedin.com/in/michaelborisov)

Issues and suggestions goes to [twitter](https://twitter.com/search?q=fluentprojections&src=typd), [issues](https://github.com/corker/fluent-projections/issues) page or [StackOverflow](http://stackoverflow.com/questions/tagged/fluent-projections).
