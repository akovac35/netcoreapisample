using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistance
{
    public interface IRepository<TDbo, TUniqueId>
        where TDbo : DboBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {

        /// <returns>Returns true is value was updated.</returns>
        /// <remarks>Database generated values are applied directly to value.</remarks>
        Task<bool> AddOrUpdateAsync(TDbo value, CancellationToken cancellationToken);

        Task<int> DeleteAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken);

        Task<TDbo?> GetAsync(Expression<Func<TDbo, bool>> predicate, CancellationToken cancellationToken);

        Task<Page<TDbo>> ListAsync(
            Expression<Func<TDbo, bool>> predicate,
            CancellationToken cancellationToken,
            int pageIndex = 0,
            int itemsPerPage = int.MaxValue,
            Func<IQueryable<TDbo>, IOrderedQueryable<TDbo>>? sorting = null);
    }
}
