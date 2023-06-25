using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class RepositoryBase<TDbo, TUniqueId, TDbContext> : IRepository<TDbo, TUniqueId>
        where TDbo : DboBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
        where TDbContext : DbContext
    {
        protected DbSet<TDbo> Queryable { get; }
        protected IDatabaseUnitOfWork DatabaseUnitOfWork { get; }

        public RepositoryBase(TDbContext dbContext, IDatabaseUnitOfWork databaseUnitOfWork)
        {
            DatabaseUnitOfWork = databaseUnitOfWork;

            Queryable = dbContext
                .Set<TDbo>();
        }

        protected virtual IQueryable<TDbo> ConfigureQueryable()
        {
            return Queryable;
        }

        public virtual async Task<bool> AddOrUpdateAsync(TDbo value, CancellationToken cancellationToken)
        {
            if (Queryable.Entry(value).IsKeySet)
            {
                _ = Queryable.Update(value);
                _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                _ = await Queryable.AddAsync(value, cancellationToken);
                _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
                return false;
            }
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken)
        {
            var nrDeleted = await Queryable.Where(predicate).ExecuteDeleteAsync(cancellationToken);
            _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
            return nrDeleted;
        }

        public virtual async Task<TDbo?> GetAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken)
        {
            var query = ConfigureQueryable();

            var dbo = await query.SingleOrDefaultAsync(predicate, cancellationToken);
            return dbo;
        }

        public virtual async Task<Page<TDbo>> ListAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken, int pageIndex = 0, int itemsPerPage = int.MaxValue, Func<IQueryable<TDbo>, IOrderedQueryable<TDbo>>? sorting = null)
        {
            var query = ConfigureQueryable().Where(predicate);

            var page = await query.ToPageAsync<TDbo, TUniqueId>(pageIndex, itemsPerPage, sorting: sorting ?? (query => query.OrderBy(dboItem => dboItem.Id)), cancellationToken: cancellationToken);
            return page;
        }
    }
}
