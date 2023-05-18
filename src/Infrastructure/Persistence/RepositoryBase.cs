using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Persistance;
using Infrastructure.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization; 

namespace Infrastructure.Persistence
{
    public class RepositoryBase<T, TDbo, TUniqueId, TDbContext> : IRepository<T, TDbo, TUniqueId>
        where T : class, IDboMappable<TUniqueId>
        where TDbo : DboBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
        where TDbContext : DbContext
    {
        private readonly IStringLocalizer localizer;

        protected DbSet<TDbo> Queryable { get; }
        protected IDboMapper<T, TDbo, TUniqueId> DboMapper { get; }
        protected IDatabaseUnitOfWork DatabaseUnitOfWork { get; }

        public RepositoryBase(IDboMapper<T, TDbo, TUniqueId> dboMapper, TDbContext dbContext, IDatabaseUnitOfWork databaseUnitOfWork, IStringLocalizerFactory localizerFactory)
        {
            DboMapper = dboMapper;
            DatabaseUnitOfWork = databaseUnitOfWork;
            localizer = localizerFactory.Create(typeof(InfrastructureText));

            // TODO LOW support configuration so includes can be specified
            Queryable = dbContext
                .Set<TDbo>();
        }

        public virtual async Task<(bool IsDbUpdated, T Instance)> AddOrUpdateAsync(T value, CancellationToken cancellationToken)
        {
            var existingDbo = await Queryable.SingleOrDefaultAsync(dboItem => IEquatable<TUniqueId>.Equals(value.DboUniqueId, dboItem.UniqueId), cancellationToken);

            if (existingDbo != null)
            {
                DboMapper.ToExistingDbo(value, ref existingDbo);
                _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
                var inst = DboMapper.FromDbo(existingDbo);
                return (true, inst);
            }
            else
            {
                var dbo = (await Queryable.AddAsync(DboMapper.ToDbo(value), cancellationToken)).Entity;
                _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
                var inst = DboMapper.FromDbo(dbo);
                return (false, inst);
            }
        }

        public virtual async Task<T> AddAsync(T value, CancellationToken cancellationToken)
        {
            var dbo = (await Queryable.AddAsync(DboMapper.ToDbo(value), cancellationToken)).Entity;
            _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
            return DboMapper.FromDbo(dbo);
        }
        public virtual async Task<T> UpdateAsync(T value, CancellationToken cancellationToken)
        {
            var existingDbo = await Queryable.SingleOrDefaultAsync(dboItem => IEquatable<TUniqueId>.Equals(value.DboUniqueId, dboItem.UniqueId), cancellationToken);

            if (existingDbo != null)
            {
                DboMapper.ToExistingDbo(value, ref existingDbo);
                _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
                var inst = DboMapper.FromDbo(existingDbo);
                return inst;
            }
            else
            {
                 throw new InvalidOperationException(localizer["DboNotFound", typeof(TDbo).FullName!, value.DboUniqueId].Value);
            }
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken)
        {
            // Approach with execute delete does not work with in-memory db
            var toDelete = await Queryable.Where(predicate).ToListAsync(cancellationToken);
            Queryable.RemoveRange(toDelete);
            _ = await DatabaseUnitOfWork.SaveChangesAsync(cancellationToken);
            return toDelete.Count;
        }

        public virtual async Task<T?> GetAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken)
        {
            var dbo = await Queryable.SingleOrDefaultAsync(predicate, cancellationToken);
            return DboMapper.FromDbo(dbo);
        }

        public virtual async Task<Page<T>> ListAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken, int pageIndex = 0, int itemsPerPage = int.MaxValue, Func<IQueryable<TDbo>, IOrderedQueryable<TDbo>>? sorting = null)
        {
            var query = Queryable.Where(predicate);

            var page = await query.ToPageAsync<T, TDbo, TUniqueId>(pageIndex, itemsPerPage, sorting: sorting ?? (query => query.OrderBy(dboItem => dboItem.Id)), mapper: DboMapper.FromDbo, cancellationToken: cancellationToken);
            return page;
        }
    }
}
