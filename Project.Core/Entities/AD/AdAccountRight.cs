using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdAccountRight : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? RightId { get; set; }
        public bool? IsAdded { get; set; }
        public bool? IsRemoved { get; set; }
    }
}
