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


2. Implement IFluentProjectionStore<TProjection>
--

IFluentProjectionStore<TProjection> is a persistence provider for your projections. It should be able to read, insert and update projections.

If you like dapper I'd be happy to say that there is an implementation for DapperFluentProjectionStore:
```
Install-Package FluentProjections.Dapper
```

3. Implement event denormalizers
--

E.g. "when a concert created add a new projection and map defined properties"

```
public class ConcertProjectionDenormalizer : FluentEventDenormalizer<ConcertProjection>
{
    private readonly IFluentProjectionStore _store;
    
    public ConcertProjectionDenormalizer(IFluentProjectionStore store)
    {
        _store = store;

        On<ConcertCreated>()
            .AddNew()
                .Do((event, projection) => {
                    projection.Id = event.ConcertId;
                    projection.ConcertDate = event.Date;
                    projection.ConcertName = event.Name;
                });
    }

    public void Handle(ConcertCreated @event) // This is a handler for ConcertCreated event
    {
        Handle(@event, _store);
    }
}
```

An incoming event can be translated into a series of other objects:

```
    On<ConcertCreated>()
        .Translate(concert => new[]
        {
            new DefineSeat { Id = concert.Seats.First().SeatId, ... },
            new DefineSeat { Id = concert.Seats.Last().SeatId, ... }
        })
        .AddNew()
            .Map(projection => projection.Id)
            .Map(projection => projection.Location, seat => seat.SeatLocation);
```

The same denormalizer can contain many event handlers. Simply register all of them in a single constructor:
```
    On<ConcertCreated>()
        .Translate(...)
        .AddNew()
            .Map(...);

    On<SeatLocationCorrected>()
        .Update()
            .FilterBy(...)
            .Map(...);
```

A signle handler can be defined for all registered events in a denormalizer:

```
    public void Handle(object @event)
    {
        Handle(@event, _store);
    }
```

This is an example of a statistics denormalizer. It counts a number of concerts created per month.
```
public class MonthStatisticsDenormalizer : FluentEventDenormalizer<MonthStatistics>
{
    private readonly IFluentProjectionStore _store;

    public MonthStatisticsDenormalizer(IFluentProjectionStore store)
    {
        _store = store;

        On<ConcertCreated>()
            .Save() // update a projection that matches provided key(s) or create a new one
                .Key(p => p.Year, e => e.Date.Year)
                .Key(p => p.Month, e => e.Date.Month)
                .Increment(p => p.Concerts);
    }

    public void Handle(object @event)
    {
        Handle(@event, _store);
    }
}
```

The library is fully covered with unit tests. Look into a FluentProjections.Tests project for more examples.

Happy coding!

What’s new?
-----------

**Nov 30th, 2013**      
Based on a feedback I had to make a significant change in a codebase:
- Handler registerer removed.
- Configurer turned into a FluentEventDenormalizer that you can inherit from and register as an event handler.

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
