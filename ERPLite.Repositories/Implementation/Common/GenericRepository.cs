using ERPLite.Data.Constants;
using ERPLite.Data.Context;
using ERPLite.Repositories.Interfaces.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERPLite.Repositories.Implementation.Common
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // ================================
        // Read Operations
        // ================================

        public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindByPredicateAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.CountAsync(cancellationToken);

            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }


        // ================================
        // Write Operations
        // ================================

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void SoftDelete(T entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsActive = false;

                _dbSet.Update(entity);

                return;
            }

            throw new InvalidOperationException(
                $"{typeof(T).Name} does not support soft delete.");
        }

        public virtual void Restore(T entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsActive = true;

                _dbSet.Update(entity);

                return;
            }

            throw new InvalidOperationException(
                $"{typeof(T).Name} does not support restore operation.");
        }
    }
}
