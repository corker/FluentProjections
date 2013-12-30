using System.Collections.Generic;
using System.Data;
using System.Linq;
using DapperExtensions;

namespace FluentProjections.Dapper
{
    /// <summary>
    ///     Implementation for <see cref="IFluentProjectionStore" /> based on Dapper.
    /// </summary>
    public class DapperFluentProjectionStore : IFluentProjectionStore
    {
        private readonly IDbTransaction _transaction;

        public DapperFluentProjectionStore(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public IEnumerable<TProjection> Read<TProjection>(IEnumerable<FluentProjectionFilterValue> values) where TProjection : class
        {
            var predicate = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = values
                    .Select(x => new FieldPredicate<TProjection>
                    {
                        PropertyName = x.Property.Name,
                        Operator = Operator.Eq,
                        Value = x.Value
                    })
                    .Cast<IPredicate>()
                    .ToList()
            };
            return _transaction.Connection.GetList<TProjection>(predicate, null, _transaction);
        }

        public void Update<TProjection>(TProjection projection) where TProjection : class
        {
            _transaction.Connection.Update(projection, _transaction);
        }

        public void Insert<TProjection>(TProjection projection) where TProjection : class
        {
            _transaction.Connection.Insert(projection, _transaction);
        }
    }
}