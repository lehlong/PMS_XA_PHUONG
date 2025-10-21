using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.MD
{
    public class MdWorkflow : BaseEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ProjectLevelCode { get; set; }
        public string? OrgId { get; set; }
        public int? Type { get; set; }
        public string? Notes { get; set; }
    }
}
