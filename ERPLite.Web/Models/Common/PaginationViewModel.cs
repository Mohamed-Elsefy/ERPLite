using ERPLite.Services.DTOs.Common;

namespace ERPLite.Web.Models.Common
{
    /// <summary>
    /// Pager state for Index views. <see cref="Filters"/> holds the current
    /// search/filter query values so page links preserve them.
    /// </summary>
    public class PaginationViewModel
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public Dictionary<string, string> Filters { get; set; } = new();

        public int FirstItemIndex => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

        public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

        public static PaginationViewModel FromPagedResponse<T>(
            PagedResponseDto<T> paged,
            Dictionary<string, string>? filters = null)
        {
            return new PaginationViewModel
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalPages = paged.TotalPages,
                TotalCount = paged.TotalCount,
                Filters = filters ?? new Dictionary<string, string>()
            };
        }
    }
}
