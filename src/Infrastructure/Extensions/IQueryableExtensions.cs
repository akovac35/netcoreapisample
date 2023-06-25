using System.Threading;
using System.Threading.Tasks;
using Domain.Persistance;
using Microsoft.EntityFrameworkCore;

namespace System.Linq
{
    public static class IQueryableExtensions
    {
        public static async Task<Page<TDbo>> ToPageAsync<TDbo, TUniqueId>(this IQueryable<TDbo> query, int pageIndex, int itemsPerPage, Func<IQueryable<TDbo>, IOrderedQueryable<TDbo>> sorting, CancellationToken cancellationToken)
            where TDbo : DboBase<TUniqueId>
            where TUniqueId : notnull, IEquatable<TUniqueId>
        {
            var items = query;
            items = sorting(items);
            var totalItems = await items.CountAsync(cancellationToken);
            // 0 - based index
            var maxPageindex = itemsPerPage > 0 && totalItems > 0 ? totalItems / itemsPerPage : 0;
            var page = new Page<TDbo>(await items.Skip(pageIndex * itemsPerPage).Take(itemsPerPage).ToListAsync(cancellationToken), pageIndex, maxPageindex, totalItems);
            return page;
        }
    }
}
