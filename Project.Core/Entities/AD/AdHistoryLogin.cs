using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.AD
{
    public class AdHistoryLogin : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public DateTime? LogonTime { get; set; }
        public string? Browser { get; set; }
        public string? Version { get; set; }
        public bool? IsMobile { get; set; }
        public string? Os { get; set; }
        public string? MobileModel { get; set; }
        public string? Manufacturer { get; set; }
        public string? IpAddress { get; set; }
        public bool? Status { get; set; }
        public string? Reason { get; set; }

    }
}
