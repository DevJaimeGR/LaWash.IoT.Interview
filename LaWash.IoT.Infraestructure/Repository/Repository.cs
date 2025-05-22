using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LaWash.IoT.Infraestructure;
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ParkingDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ParkingDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await SaveChangesAsync();
    }
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveChangesAsync();
    }
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<T?> FindNoTrackingAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

    public async Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>> filter,
            PaginationParams pagination,
            params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = query.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
}

