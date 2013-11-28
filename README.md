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
public class ConcertSeatProjectionConfiguration : FluentProjectionConfiguration<ConcertSeatProjection>
{
    public ConcertSeatProjectionConfiguration()
    {
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
}

public class MonthStatisticsConfiguration : FluentProjectionConfiguration<MonthStatistics>
{
    public MonthStatisticsConfiguration()
    {
        ForEvent<ConcertCreated>()
            .Save() // update a projection that matches provided key(s) in a store or create a new one
                .Key(p => p.Year, e => e.Date.Year)
                .Key(p => p.Month, e => e.Date.Month)
                .Increment(p => p.Concerts);
    }
}
```

3. Implement IFluentProjectionStore<TProjection>
--

IFluentProjectionStore<TProjection> is a persistence provider for your projections. It should be able to read, insert and update projections.

Later I'm going to provide implementation for different database types, but right now it's not there.

4. Implement IFluentEventHandlerRegisterer
--

IFluentEventHandlerRegisterer is a bridge between FluentProjections and your event bus. I don't know what event bus you use, so it's up to you how to register handlers in your code.

E.g. NServiceBus uses IHandleMessages<TMessage> interface. I can imagine that you write a generic event handler class that implements IHandleMessages<TMessage> interface and get IFluentEventHandler as an argument in a constructor.

In this case implementation for registerer should create that generic event handler, initialize it with fluent event handler and register generic event handler in your IoC container.

Later I'm going to provide implementation for different event sourcing frameworks, but right now it's not there.

5. Register your projection configurations
--

Here is an example:

```
public class ApplicationBootstrap {

    public Bootstrap() {
        var iocContainer = ...
        
        // create a registerer that register fluent event handlers in a container
        var registerer = new MyFluentEventHandlerRegisterer(iocContainer);
        
        // register fluent event handlers that constructed by a configuration
        new MonthStatisticsConfiguration().RegisterBy(registerer);
        
        ...
    }
}
```

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
