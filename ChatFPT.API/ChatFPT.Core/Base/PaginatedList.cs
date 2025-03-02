using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Core.Pagination
{
    public class PaginatedList<T>
    {
        public IReadOnlyCollection<T>? Items { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public PaginatedList(IReadOnlyCollection<T>? items, int pageNumber, int pageSize, int count)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
            Items = items;
        }

        public bool PreviousPage => PageNumber > 1;
        public bool NextPage => PageNumber < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
