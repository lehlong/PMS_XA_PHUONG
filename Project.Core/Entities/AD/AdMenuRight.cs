using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdMenuRight : BaseEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? MenuId { get; set; }
        public string? RightId { get; set; }
    }
}
