using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistance
{
    public interface IRepository<T, TDbo, TUniqueId>
        where T : class, IDboMappable<TUniqueId>
        where TDbo : DboBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        /// <returns>
        /// The returned instance is a new value mapped from dbo
        /// after applying database generated values.
        /// </returns>
        Task<(bool IsDbUpdated, T Instance)> AddOrUpdateAsync(T value, CancellationToken cancellationToken);

        /// <returns>
        /// The returned instance is a new value mapped from dbo
        /// after applying database generated values.
        /// </returns>
        Task<T> AddAsync(T value, CancellationToken cancellationToken);

        /// <returns>
        /// The returned instance is a new value mapped from dbo
        /// after applying database generated values.
        /// </returns>
        Task<T> UpdateAsync(T value, CancellationToken cancellationToken);

        Task<int> DeleteAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken);

        Task<T?> GetAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken);

        Task<Page<T>> ListAsync(
            Expression<Func<TDbo, bool>> predicate,
            CancellationToken cancellationToken,
            int pageIndex = 0,
            int itemsPerPage = int.MaxValue,
            Func<IQueryable<TDbo>, IOrderedQueryable<TDbo>>? sorting = null);
    }
}
