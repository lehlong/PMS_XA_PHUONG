using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdMessage : SoftDeleteEntity
    {
        [Key]
        public string Code { get; set; } = string.Empty;
        public string? Lang { get; set; }
        public string? Message { get; set; }
        public string? Type { get; set; }
    }
}
