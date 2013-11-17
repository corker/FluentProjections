fluent-projections
==================

Fluent Projections provides a configurable hub that can handle events and map them to database projections (read models).

Why?
====

When CQRS and Event Sourcing come in, you have to deal with projections or read models. Projection contains data prepared for a specific view. To process events we use so called denormalizers. Denormalizer is a hub that handle domain evens and map them to a projection. To define a mapping between events and a projection I have to write a very similar code.

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

Later I'm going to provide implementation for different database types, but right now it's not there.

3. Implement IFluentEventHandlerRegisterer
--

IFluentEventHandlerRegisterer is a bridge between FluentProjections and your event bus. I don't know what event bus you use, so it's up to you how to register handlers in your code.

E.g. NServiceBus uses IHandleMessages<TMessage> interface. I can imagine that you write a generic event handler class that implements IHandleMessages<TMessage> interface and get IFluentEventHandler as an argument in a constructor.

In this case implementation for registerer should create that generic event handler, initialize it with fluent event handler and register generic event handler in your IoC container.

Later I'm going to provide implementation for different event sourcing frameworks, but right now it's not there.

4. Implement your projection configurations
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

5. Register your projection configurations
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

What’s new?
-----------

**Nov 17th, 2013**      
A single event can be translated into a series of objects.
Translated objects can be configured to be handled the same way as oher events.
See tests.

Who am I?
--
My name is Michael Borisov. I'm interested in CQRS, event sourcing and service oriented architecture.

If you have any comments please feel free to contact me on [Twitter](https://twitter.com/fkem) or [LinkedIn](https://www.linkedin.com/in/michaelborisov)

Issues and suggestions goes to [twitter](https://twitter.com/search?q=fluentprojections&src=typd), [issues](https://github.com/corker/fluent-projections/issues) page or [StackOverflow](http://stackoverflow.com/questions/tagged/fluent-projections).
