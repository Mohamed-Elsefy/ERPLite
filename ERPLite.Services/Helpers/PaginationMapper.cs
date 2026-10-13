using AutoMapper;
using ERPLite.Repositories.Extensions;
using ERPLite.Services.DTOs.Common;

namespace ERPLite.Services.Helpers
{
    /// <summary>
    /// Bridges repository-level <see cref="PaginationResult{T}"/> (entities)
    /// to service-level <see cref="PagedResponseDto{T}"/> (DTOs).
    /// </summary>
    public static class PaginationMapper
    {
        public static PagedResponseDto<TDestination> ToPagedResponse<TSource, TDestination>(
            this PaginationResult<TSource> paged,
            IMapper mapper)
        {
            return new PagedResponseDto<TDestination>
            {
                Items = mapper.Map<List<TDestination>>(paged.Items),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages
            };
        }
    }
}
