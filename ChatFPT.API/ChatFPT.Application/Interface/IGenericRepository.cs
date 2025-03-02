

using ChatFPT.Core.Pagination;

namespace ChatFPT.Application.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<PaginatedList<T>> GetPagingAsync(IQueryable<T> query, int pageIndex, int pageSize);

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
    }
}
