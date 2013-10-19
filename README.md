fluent-projections
==================

Fluent Projections provides a configurable hub that can handle events and map them to database projections (read models).

Why?
====

When CQRS and Event Sourcing come in you have to deal with projections, or read models. Projections are used to prepare data for a user.

To do that we use so called denormalizers - hubs that can handle domain evens and map it to projections.

I was bored writing the same code for every projection again and again with a single reason - to define mapping between events and projections.

I noticed some patterns:
* Direct mapping to a projection property with another or even the same name;
* All kind of aggregations;
* All kind of lookups.

So as I'm a big fan of configuration I want to share with you the idea of configurable projection denormalizers.

How?
====

Currently there is not much you can find - only direct mapping of properties, and there is no concrete persistence implementations, so stay tuned.

To be able to use FluentProjections you have to do the following steps.

1. Implement an IFluentProjectionStore<TProjection> interface.
--

This is a persistence provider for your projections. It should be able to read, insert and update projections.

Later I'm going to provide implementations out of the box for different databases, but right now it's not there.

2. Implement an IFluentEventHandlerRegisterer interface.
--

This is a bridge between FluentProjections and your event bus. I don't know what event bus implementation you using, so it's up to you how to register handlers in your code.

E.g. NServiceBus uses an IHandleMessages<TMessage> intervace. I can imagine that you write a generic event handler class that implements this interface and get an IFluentEventHandler as an argument in it's constructor.

In this case implementation for registerer should create that generic event handler, initialize it with fluent event handler and register it in your IoC container.

Later I'm going to provide implementations out of the box for different event sourcing frameworks, but right now it's not there.

3. Implement your projection configurations.
--

Here is an example:

```
public TestConfiguration()
{
    ForEvent<TestEvent>()
        .AddNew()
            .Map(p => p.ValueBoolean, e => e.ValueBoolean)
            .Map(p => p.ValueDecimal, e => e.ValueDecimal)
            .Map(p => p.ValueInt32, e => e.ValueInt32)
            .Map(p => p.ValueString, e => e.ValueString);
        }
```

4. Register your projection configurations.
--

Here is an example:

```
_targetRegisterer = new TestRegisterer();

new TestConfiguration().RegisterBy(_targetRegisterer);
```

On a project web site you can find a test project where more examples can be found.

Happy coding!
