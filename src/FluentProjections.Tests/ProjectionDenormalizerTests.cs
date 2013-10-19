using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace FluentProjections.Tests
{
    public class ProjectionDenormalizerTests
    {
        private class TestEvent
        {
            public bool ValueBoolean { get; set; }
            public byte ValueByte { get; set; }
            public sbyte ValueSByte { get; set; }
            public char ValueChar { get; set; }
            public decimal ValueDecimal { get; set; }
            public double ValueDouble { get; set; }
            public float ValueSingle { get; set; }
            public int ValueInt32 { get; set; }
            public uint ValueUInt32 { get; set; }
            public long ValueInt64 { get; set; }
            public ulong ValueUInt64 { get; set; }
            public short ValueInt16 { get; set; }
            public ushort ValueUInt16 { get; set; }
            public string ValueString { get; set; }
        }

        private class TestProjection
        {
            public bool ValueBoolean { get; set; }
            public byte ValueByte { get; set; }
            public sbyte ValueSByte { get; set; }
            public char ValueChar { get; set; }
            public decimal ValueDecimal { get; set; }
            public double ValueDouble { get; set; }
            public float ValueSingle { get; set; }
            public int ValueInt32 { get; set; }
            public uint ValueUInt32 { get; set; }
            public long ValueInt64 { get; set; }
            public ulong ValueUInt64 { get; set; }
            public short ValueInt16 { get; set; }
            public ushort ValueUInt16 { get; set; }
            public string ValueString { get; set; }
        }

        private class TestRegisterer : IFluentEventHandlerRegisterer
        {
            public IFluentEventHandler<TestEvent, TestProjection> Handler { get; private set; }

            public void Register<TEvent, TProjection>(IFluentEventHandler<TEvent, TProjection> eventHandler)
            {
                Handler = (IFluentEventHandler<TestEvent, TestProjection>) eventHandler;
            }
        }


        private class TestStore : IFluentProjectionStore<TestProjection>
        {
            public TestProjection InsertProjection { get; private set; }

            public IEnumerable<TestProjection> Read(FluentProjectionFilterValues values)
            {
                throw new NotImplementedException();
            }

            public void Update(TestProjection projection)
            {
                throw new NotImplementedException();
            }

            public void Insert(TestProjection projection)
            {
                InsertProjection = projection;
            }
        }

        [TestFixture]
        public class When_event_adds_new_projection_with_simple_mappings
        {
            private TestProjection _targetProjection;
            private TestRegisterer _targetRegisterer;

            private class TestConfiguration : FluentProjectionConfiguration<TestProjection>
            {
                public TestConfiguration()
                {
                    ForEvent<TestEvent>()
                        .AddNew()
                        .Map(p => p.ValueBoolean, e => e.ValueBoolean)
                        .Map(p => p.ValueByte, e => e.ValueByte)
                        .Map(p => p.ValueSByte, e => e.ValueSByte)
                        .Map(p => p.ValueChar, e => e.ValueChar)
                        .Map(p => p.ValueDecimal, e => e.ValueDecimal)
                        .Map(p => p.ValueDouble, e => e.ValueDouble)
                        .Map(p => p.ValueSingle, e => e.ValueSingle)
                        .Map(p => p.ValueInt32, e => e.ValueInt32)
                        .Map(p => p.ValueUInt32, e => e.ValueUInt32)
                        .Map(p => p.ValueInt64, e => e.ValueInt64)
                        .Map(p => p.ValueUInt64, e => e.ValueUInt64)
                        .Map(p => p.ValueInt16, e => e.ValueInt16)
                        .Map(p => p.ValueUInt16, e => e.ValueUInt16)
                        .Map(p => p.ValueString, e => e.ValueString);
                }
            }

            [TestFixtureSetUp]
            public void Init()
            {
                _targetRegisterer = new TestRegisterer();

                new TestConfiguration().RegisterBy(_targetRegisterer);

                var @event = new TestEvent
                {
                    ValueBoolean = true,
                    ValueByte = 1,
                    ValueSByte = 2,
                    ValueChar = 'c',
                    ValueDecimal = 3,
                    ValueDouble = 4,
                    ValueSingle = 5,
                    ValueInt32 = 6,
                    ValueUInt32 = 7,
                    ValueInt64 = 8,
                    ValueUInt64 = 9,
                    ValueInt16 = 10,
                    ValueUInt16 = 11,
                    ValueString = "s"
                };

                var store = new TestStore();

                _targetRegisterer.Handler.Handle(@event, store);

                _targetProjection = store.InsertProjection;
            }

            [Test]
            public void Should_map_boolean_value()
            {
                Assert.True(_targetProjection.ValueBoolean);
            }

            [Test]
            public void Should_map_byte_value()
            {
                Assert.AreEqual(1, _targetProjection.ValueByte);
            }

            [Test]
            public void Should_map_char_value()
            {
                Assert.AreEqual('c', _targetProjection.ValueChar);
            }

            [Test]
            public void Should_map_decimal_value()
            {
                Assert.AreEqual(3, _targetProjection.ValueDecimal);
            }

            [Test]
            public void Should_map_double_value()
            {
                Assert.AreEqual(4, _targetProjection.ValueDouble);
            }

            [Test]
            public void Should_map_int16_value()
            {
                Assert.AreEqual(10, _targetProjection.ValueInt16);
            }

            [Test]
            public void Should_map_int32_value()
            {
                Assert.AreEqual(6, _targetProjection.ValueInt32);
            }

            [Test]
            public void Should_map_int64_value()
            {
                Assert.AreEqual(8, _targetProjection.ValueInt64);
            }

            [Test]
            public void Should_map_sbyte_value()
            {
                Assert.AreEqual(2, _targetProjection.ValueSByte);
            }

            [Test]
            public void Should_map_single_value()
            {
                Assert.AreEqual(5, _targetProjection.ValueSingle);
            }

            [Test]
            public void Should_map_string_value()
            {
                Assert.AreEqual("s", _targetProjection.ValueString);
            }

            [Test]
            public void Should_map_uint16_value()
            {
                Assert.AreEqual(11, _targetProjection.ValueUInt16);
            }

            [Test]
            public void Should_map_uint32_value()
            {
                Assert.AreEqual(7, _targetProjection.ValueUInt32);
            }

            [Test]
            public void Should_map_uint64_value()
            {
                Assert.AreEqual(9, _targetProjection.ValueUInt64);
            }
        }
    }
}