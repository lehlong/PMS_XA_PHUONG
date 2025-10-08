using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdAccountGroup : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? OrgId { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

}
