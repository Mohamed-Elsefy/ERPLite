using System.Linq.Expressions;

namespace ERPLite.Repositories.Interfaces.Common
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        // ================================
        // Read Operations (no tracking unless stated)
        // ================================

        /// <summary>
        /// Tracked lookup by key. Prefer this when the entity will be updated;
        /// use concrete repository read methods for display-only scenarios.
        /// </summary>
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> FindByPredicateAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read-only queryable surface. Prefer concrete repository methods with explicit
        /// Includes over composing queries in services.
        /// </summary>
        IQueryable<T> GetQueryable();

        // ================================
        // Write Operations
        // ================================

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        void Update(T entity);

        void Delete(T entity);

        void SoftDelete(T entity);

        void Restore(T entity);
    }
}
