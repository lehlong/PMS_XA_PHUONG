using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.MD
{
    public class MdWorkflowStep : BaseEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? WorkflowId { get; set; }
        public int? Step { get; set; }
        public string? Name { get; set; }
        public int? HanXuLy { get; set; }
        public int? Action { get; set; }
        public virtual MdWorkflow? Workflow { get; set; }
    }
}
