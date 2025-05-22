using System.Linq.Expressions;

namespace LaWash.IoT.Infraestructure;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task SaveChangesAsync();
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindNoTrackingAsync(Expression<Func<T, bool>> predicate);
    Task<PagedResult<T>> GetPagedAsync(
    Expression<Func<T, bool>> filter,
    PaginationParams pagination,
    params Expression<Func<T, object>>[] includes);
}

