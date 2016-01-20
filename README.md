fluent-projections
==================

This package provides a configurable hub that can handle events and map them onto event projection tables (read models).

Why?
====

When CQRS and Event Sourcing come in place, we have to deal with event projections also known as read models. In most cases a projection contains a data model prepared for a specific view and optimized for querying. To map events onto projections we use so called event denormalizers. Implementation for denormalizers is a trivial and time consuming task.

What?
====

With this project I want to minimize an effort on defining event denormalizers by providing a semantic way of doing it. I beleive it's great if I can type "On a concert created event add a new projection with a concert date and title" and a computer could translate it into a code. What do you think?

The codebase was split by components:

[FluentProjections](https://github.com/corker/FluentProjections)
[FluentProjections.AutoMapper](https://github.com/corker/FluentProjections.AutoMapper)
[FluentProjections.ValueInjecter](https://github.com/corker/FluentProjections.ValueInjecter)
[FluentProjections.Persistence.SQL](https://github.com/corker/FluentProjections.Persistence.SQL)
