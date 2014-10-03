using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FluentProjections.EntityFramework
{
    /// <summary>
    ///     Implementation for <see cref="IFluentProjectionStore" /> based on EntityFramework version 6.
    /// </summary>
    public class EntityFrameworkFluentProjectionStore : IFluentProjectionStore
    {
        private readonly DbContext _context;

        public EntityFrameworkFluentProjectionStore(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<TProjection> Read<TProjection>(IEnumerable<FluentProjectionFilterValue> values)
            where TProjection : class
        {
            return QueryProjections<TProjection>(values);
        }

        public void Update<TProjection>(TProjection projection) where TProjection : class
        {
            // No updates required. Entity Framework implements unit of work pattern.
            // All changes persisted a call to SaveChanges.
        }

        public void Insert<TProjection>(TProjection projection) where TProjection : class
        {
            _context.Set<TProjection>().Add(projection);
        }

        public void Remove<TProjection>(IEnumerable<FluentProjectionFilterValue> values) where TProjection : class
        {
            IQueryable<TProjection> projections = QueryProjections<TProjection>(values);
            _context.Set<TProjection>().RemoveRange(projections);
        }

        private IQueryable<TProjection> QueryProjections<TProjection>(IEnumerable<FluentProjectionFilterValue> values)
            where TProjection : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof (TProjection), "projection");
            BinaryExpression expression = null;
            foreach (FluentProjectionFilterValue value in values)
            {
                MemberExpression property = Expression.Property(parameter, value.Property);
                BinaryExpression equal = Expression.Equal(property, Expression.Constant(value.Value));
                expression = expression == null ? equal : Expression.And(expression, equal);
            }
            if (expression == null)
            {
                return _context.Set<TProjection>();
            }
            var lambda = Expression.Lambda<Func<TProjection, bool>>(expression, parameter);
            return _context.Set<TProjection>().Where(lambda);
        }
    }
}