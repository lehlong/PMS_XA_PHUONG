using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdAccount : SoftDeleteEntity
    {
        [Key]
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? OrgId { get; set; }
    }
}
