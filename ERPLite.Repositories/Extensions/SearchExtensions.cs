using System.Linq.Expressions;

namespace ERPLite.Repositories.Extensions
{
    public static class SearchExtensions
    {
        public static IQueryable<T>  ApplySearch<T>(
                this IQueryable<T> query,
                Expression<Func<T, bool>> predicate)
        {
            return query.Where(predicate);
        }
    }
}
