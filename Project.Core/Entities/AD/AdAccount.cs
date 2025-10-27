using Project.Core.Common;
using Project.Core.Entities.MD;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdAccount : BaseEntity
    {
        [Key]
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? OrgId { get; set; }
        public string? TitleCode { get; set; }
        public virtual MdTitle? Title { get; set; }
    }
}
