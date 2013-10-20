fluent-projections
==================

Fluent Projections provides a configurable hub that can handle events and map them to database projections (read models).

Why?
====

When CQRS and Event Sourcing come in, you have to deal with projections or read models. Projection contains data prepared for specific view.

To process events we use so called denormalizers. Denormalizer is a hub that handle domain evens and map them to projections.

I was bored writing the same code for every projection again and again with a single reason - to define mapping between event and projection.

What?
====

So, I noticed some patterns in this process:
* Direct mapping to a projection property with another or even the same name;
* All kind of aggregations;
* All kind of lookups.

I'm a big fan of configuring and I want to share with you the idea of configurable projection denormalizers that I call Fluent Projections.

How?
====

There is not much you can do right now - only map properties directly. And there is no concrete persistence implementations. So stay tuned.

To be able to use FluentProjections you have to do the following steps.

1. Implement IFluentProjectionStore<TProjection> interface
--

IFluentProjectionStore<TProjection> is a persistence provider for your projections. It should be able to read, insert and update projections.

Later I'm going to provide implementation for different database types, but right now it's not there.

2. Implement IFluentEventHandlerRegisterer interface
--

IFluentEventHandlerRegisterer is a bridge between FluentProjections and your event bus. I don't know what event bus you use, so it's up to you how to register handlers in your code.

E.g. NServiceBus uses IHandleMessages<TMessage> interface. I can imagine that you write a generic event handler class that implements IHandleMessages<TMessage> interface and get IFluentEventHandler as an argument in a constructor.

In this case implementation for registerer should create that generic event handler, initialize it with fluent event handler and register generic event handler in your IoC container.

Later I'm going to provide implementation for different event sourcing frameworks, but right now it's not there.

3. Implement your projection configurations
--

Here is an example:

```
public class TestConfiguration : FluentProjectionConfiguration<TestProjection>
{
    public TestConfiguration()
    {
        ForEvent<TestEvent>()
            .AddNew()
            .Map(p => p.ValueInt32, e => e.ValueInt32);
    }
}
```

4. Register your projection configurations
--

Here is an example:

```
public class ApplicationBootstrap {

    public Bootstrap() {
        var iocContainer = ...
        
        _targetRegisterer = new TestRegisterer(iocContainer);
        new TestConfiguration().RegisterBy(_targetRegisterer);
        
        ...
    }
}
```

On a project website you can find a test project where more examples can be observed.

Happy coding!
