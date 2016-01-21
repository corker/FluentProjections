FluentProjections
==================

FluentProjections is a configurable hub that can map messages onto projections and store them into a database of your choice.

Why?
====

When CQRS and Event Sourcing come in place, we have to deal with event projections also known as read models. In most cases a projection contains a data model prepared for a specific view and optimized for querying. To map events onto projections we use so called event denormalizers. Implementation for denormalizers is a trivial and time consuming task.

What?
====

With this project I want to minimize an effort on defining event denormalizers by providing a semantic way of doing it. I beleive it's great if I can type "On a concert created event add a new projection with a concert date and title" and a computer could translate it into a code. What do you think?

[FluentProjections](https://github.com/corker/FluentProjections)
- The main package. An implementation for a generic message handler.

[FluentProjections.AutoMapper](https://github.com/corker/FluentProjections.AutoMapper)
- An extension to automatically map message properties to a projection using AutoMapper package.

[FluentProjections.ValueInjecter](https://github.com/corker/FluentProjections.ValueInjecter)
- An extension to automatically map message properties to a projection using ValueInjecter package.

[FluentProjections.Persistence.SQL](https://github.com/corker/FluentProjections.Persistence.SQL) - An extension to store projections into a relational database using Dapper and DapperExtension packages.
