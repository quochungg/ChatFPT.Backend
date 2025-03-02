namespace ChatFPT.Application.Repositories;

using global::ChatFPT.Application.Interface;
using global::ChatFPT.Core.Pagination;
using global::ChatFPT.Insfracstructure.Base;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ChatBoxDBContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ChatBoxDBContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<PaginatedList<T>> GetPagingAsync(IQueryable<T> query, int pageIndex, int pageSize)
    {
        return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
    }
}
    

