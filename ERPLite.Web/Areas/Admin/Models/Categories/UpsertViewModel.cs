using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Areas.Admin.Models.Categories
{
    public class UpsertViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
