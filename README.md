FluentProjections [![Build status](https://ci.appveyor.com/api/projects/status/kdt0pv546527w4kc?svg=true)](https://ci.appveyor.com/project/corker/fluentprojections) [![NuGet Status](http://img.shields.io/nuget/v/FluentProjections.svg?style=flat)](https://www.nuget.org/packages/FluentProjections/) [![Join the chat at https://gitter.im/corker/FluentProjections](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/corker/FluentProjections?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
==================


This package provides a configurable hub that can handle events and map them onto event projection tables (read models).

Why?
====

When CQRS and Event Sourcing come in place, we have to deal with event projections also known as read models. In most cases a projection contains a data model prepared for a specific view and optimized for querying. To map events onto projections we use so called event denormalizers. Implementation for denormalizers is a trivial and time consuming task.

What?
====

With this project I want to minimize an effort on defining event denormalizers by providing a semantic way of doing it. I beleive it's great if I can type "On a concert created event add a new projection with a concert date and title" and a computer could translate it into a code. What do you think?

How?
====

1. Install a nuget package
--

```
Install-Package FluentProjections
```

2. Implement message handlers
--

To implement a message handler inherit from MessageHandler<_ProjectionClassName_> and define a behavior for messages that have to be projected like it's shown below:

```
/// <summary>
///    "On a concert created event add a new projection with a concert date and title"
/// </summary> 
public class ConcertMessageHandler : MessageHandler<ConcertProjection>
{
    public ConcertMessageHandler(ICreateProjectionProviders factory) : base(factory)
    {
    }

    public void Handle(ConcertCreated message) // This is a handler for ConcertCreated event
    {
        Handle(message, m => m
            .AddNew()
            .Map(x => x.Id)
            .Map(x => x.ConcertDate)
            .Map(x => x.ConcertTitle));
    }
}
```

An incoming event can be translated into a series of other objects:

```
    Handle(message, x => x
        .Translate(concert => new[]
        {
            new DefineSeat { Id = concert.Seats.First().SeatId, ... },
            new DefineSeat { Id = concert.Seats.Last().SeatId, ... }
        })
        .AddNew()
        .Map(x => x.Id)
        .Map(x => x.Location, seat => seat.SeatLocation));
```

The same class can contain multiple event handlers:
```
public class ConcertMessageHandler : MessageHandler<ConcertProjection>
{
    public ConcertMessageHandler(ICreateProjectionProviders factory) : base(factory)
    {
    }

    public void Handle(ConcertCreated message)
    {
        Handle(message, x => x
            .AddNew()
            .Map(x => x.Id)
            .Map(x => x.ConcertDate)
            .Map(x => x.ConcertTitle));
    }

    public void Handle(SeatLocationCorrected message)
    {
        Handle(message, x => x
            .Update()
            .WhenEqual(...)
            .Map(...));
    }
}
```

3. Register an implementaton for ICreateProjectionProviders in IOC container.
--

ICreateProjectionProviders is a persistence provider for projections. It reads, inserts, updates and deletes projections.

There is an implementation for sql-like databases available out of the box. It's build on top of Dapper and DapperExtensions packages. Thus it supports all the databases supported by these packages.
```
Install-Package FluentProjections.Persistence.SQL
```

Component registration for Autofac could look like this:
```
builder.Register<ICreateProjectionProviders>(x => 
        new SqlPersistenceFactory(() => new SQLiteConnection(ConnectionString))
        );
```

More examples can be found here [FluentProjections.Examples](https://github.com/corker/FluentProjections.Examples)

4. Explore extensions
--
Next to the main package you can try the following extensions:

[FluentProjections.AutoMapper](https://github.com/corker/FluentProjections.AutoMapper)
> An extension to automatically map message properties to a projection using AutoMapper package.

[FluentProjections.ValueInjecter](https://github.com/corker/FluentProjections.ValueInjecter)
> An extension to automatically map message properties to a projection using ValueInjecter package.

[FluentProjections.Persistence.SQL](https://github.com/corker/FluentProjections.Persistence.SQL)
> An extension to store projections into a relational database using Dapper and DapperExtension packages.


What’s new?
-----------

**2.10.0 January 20th, 2016**      
- Fully functional version including SQL support.

Who am I?
--
My name is Michael Borisov. I'm interested in CQRS, DDD, event sourcing and micro services architecture.

If you have any questions or comments regarding to the project please feel free to contact me on [Twitter](https://twitter.com/fkem) or [LinkedIn](https://www.linkedin.com/in/michaelborisov)

Happy coding!
