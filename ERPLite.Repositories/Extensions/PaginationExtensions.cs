using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationResult<T>> ToPagedResultAsync<T>(
                this IQueryable<T> query,
                int pageNumber,
                int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalCount =
                await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                        totalCount / (double)pageSize)
            };
        }
    }
}
