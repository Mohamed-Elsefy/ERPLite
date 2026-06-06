using System.Linq.Expressions;

namespace ERPLite.Repositories.Interfaces.Common
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        // ================================
        // Read Operations
        // ================================

        Task<T?> GetByIdAsync(TKey id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task <IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate);

        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate);

        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null);

        IQueryable<T> GetQueryable();

        // ================================
        // Write Operations
        // ================================

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);

        void SoftDelete(T entity);

        void Restore(T entity);
    }
}
