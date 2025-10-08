using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdRight : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? PId { get; set; }
        public int? OrderNumber { get; set; }
        public bool? Expanded { get; set; }
    }
}
