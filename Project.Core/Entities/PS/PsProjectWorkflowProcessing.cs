using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.PS
{
    public class PsProjectWorkflowProcessing : BaseEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public int? Step { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public int? HanXuLy { get; set; }
        public int? Action { get; set; }
        public bool? IsDone { get; set; }
        public bool? IsProcessing { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
