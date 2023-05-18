using System.Collections.Generic;

namespace Domain.Persistance
{
    public class Page<T>
    {
        public Page(IReadOnlyList<T> items, int pageIndex, int maxPageIndex, int totalItems)
        {
            Items = items;
            PageIndex = pageIndex;
            MaxPageIndex = maxPageIndex;
            TotalItems = totalItems;
        }

        public IReadOnlyList<T> Items { get; }
        public int PageIndex { get; }
        public int MaxPageIndex { get; }
        public int TotalItems { get; }
    }
}
