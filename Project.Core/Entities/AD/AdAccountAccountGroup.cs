using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdAccountAccountGroup : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? GroupId { get; set; }
    }
}
