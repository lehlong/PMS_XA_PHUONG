using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.PS
{
    public class PsProjectPerson : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? ProjectRoleCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual PsProject? Project { get; set; }
    }
}
