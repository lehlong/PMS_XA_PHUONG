using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.MD
{
    public class MdCustomer : BaseEntity
    {
        [Key]
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? MaSoThue { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }

    }
}
