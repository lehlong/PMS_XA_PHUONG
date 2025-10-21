using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.MD
{
    public class MdUnit : BaseEntity
    {
        [Key]
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Notes { get; set; }
    }
}
