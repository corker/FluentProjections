using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using NUnit.Framework;

namespace FluentProjections.EntityFramework.Tests
{
    public class EntityFrameworkFluentProjectionStoreTests
    {
        private const string TestDatabaseConnection =
            @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\FluentProjections.EntityFramework.Tests.mdf;Integrated Security=True;";

        private class TestDbContext : DbContext
        {
            public TestDbContext() : base(TestDatabaseConnection)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<TestProjection>();
            }
        }

        private class TestEvent
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
        }

        [Table("TestProjection")]
        private class TestProjection
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
        }

        [TestFixture]
        public class When_add_new
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .AddNew()
                        .Map(p => p.Id)
                        .Map(p => p.Field1));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            [Test]
            public void shoud_add_projection()
            {
                using (new TransactionScope())
                {
                    //Arrange
                    var context = new TestDbContext();
                    var store = new EntityFrameworkFluentProjectionStore(context);
                    var handler = new TestDenormalizer(store);

                    //Act
                    handler.Handle(new TestEvent {Id = 1, Field1 = "Field1"});
                    context.SaveChanges();

                    //Assert
                    Assert.AreEqual("Field1", context.Set<TestProjection>().Single().Field1);
                }
            }
        }

        [TestFixture]
        public class When_remove
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Remove()
                        .WhenEqual(p => p.Field1, p => p.Field1));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            [Test]
            public void shoud_remove_projection()
            {
                using (new TransactionScope())
                {
                    //Arrange
                    var context = new TestDbContext();
                    context.Set<TestProjection>().Add(new TestProjection {Field1 = "Field1"});
                    context.SaveChanges();

                    var store = new EntityFrameworkFluentProjectionStore(context);
                    var handler = new TestDenormalizer(store);

                    //Act
                    handler.Handle(new TestEvent {Field1 = "Field1"});
                    context.SaveChanges();

                    //Assert
                    Assert.False(context.Set<TestProjection>().Any());
                }
            }
        }

        [TestFixture]
        public class When_update
        {
            private class TestDenormalizer : FluentEventDenormalizer<TestProjection>
            {
                private readonly IFluentProjectionStore _store;

                public TestDenormalizer(IFluentProjectionStore store)
                {
                    _store = store;

                    On<TestEvent>(x => x
                        .Update()
                        .WhenEqual(p => p.Id, p => p.Id)
                        .Map(p => p.Field1));
                }

                public void Handle(TestEvent @event)
                {
                    Handle(@event, _store);
                }
            }

            [Test]
            public void shoud_query_projection()
            {
                using (new TransactionScope())
                {
                    //Arrange
                    var context = new TestDbContext();
                    var projection = new TestProjection();
                    context.Set<TestProjection>().Add(projection);
                    context.SaveChanges();

                    var store = new EntityFrameworkFluentProjectionStore(context);
                    var handler = new TestDenormalizer(store);

                    //Act
                    handler.Handle(new TestEvent {Id = projection.Id, Field1 = "Field1"});
                    context.SaveChanges();

                    //Assert
                    Assert.AreEqual("Field1", context.Set<TestProjection>().Single().Field1);
                }
            }
        }
    }
}