using System.Linq.Expressions;

namespace ERPLite.Repositories.Extensions
{
    public static class SortingExtensions
    {
        public static IQueryable<T> ApplySorting<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending = false)
        {
            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }
    }
}
