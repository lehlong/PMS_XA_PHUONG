using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdAccountGroupRight : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? GroupId { get; set; }
        public string? RightId { get; set; }
    }
}
