using ERPLite.Services.DTOs.System;

namespace ERPLite.Web.Areas.Admin.Models.System
{
    public class ActivityLogsIndexViewModel
    {
        public IEnumerable<ActivityLogDto> Logs { get; set; } = new List<ActivityLogDto>();
        public string? SelectedUser { get; set; }
        public string? SelectedModule { get; set; }
    }
}
