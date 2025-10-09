using Project.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.MD
{
    public class MdCustomer : SoftDeleteEntity
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
