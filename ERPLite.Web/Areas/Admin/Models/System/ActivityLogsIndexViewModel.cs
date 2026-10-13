using ERPLite.Services.DTOs.System;
using ERPLite.Web.Models.Common;

namespace ERPLite.Web.Areas.Admin.Models.System
{
    public class ActivityLogsIndexViewModel
    {
        public IEnumerable<ActivityLogDto> Logs { get; set; } = new List<ActivityLogDto>();
        public string? SelectedUser { get; set; }
        public string? SelectedModule { get; set; }
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
