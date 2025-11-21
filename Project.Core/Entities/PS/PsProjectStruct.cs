using Project.Core.Common;
using Project.Core.Entities.CM;
using Project.Core.Entities.MD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Core.Entities.PS
{
    public class PsProjectStruct : BaseEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? PId { get; set; }

        public string? WorkflowId { get; set; }
        public string? CurrentStepWorkflowId { get; set; }
        public int? Status { get; set; }
        public int? OrderNumber { get; set; }
        public bool? Expanded { get; set; }
        public string? OrgId { get; set; }
        public int? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? RefrenceFileId { get; set; }
        public string? Notes { get; set; }
        public virtual PsProject? Project { get; set; }
        public virtual MdWorkflow? Workflow { get; set; }
        public virtual ICollection<CmFile>? Files { get; set; }
        public virtual ICollection<PsTaskPerson>? TaskPerson { get; set; }
    }
    public class PsTaskPerson : BaseEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? TaskId { get; set; } = string.Empty;
        [ForeignKey("TaskId")]
        [JsonIgnore]
        public virtual PsProjectStruct? PsProjectStruct { get; set; }
        public string ProjectId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? TaskRoles { get; set; }
        public string? ProjectRoleCode { get; set; }
        public virtual ICollection<PsTaskPersonDetail>? TaskPersonDetails { get; set; }
    }
    public class PsTaskPersonDetail : BaseEntity
    {
        [Key]
        public string? Id { get; set; } = string.Empty;
        public string? TaskPersonId { get; set; } = string.Empty;
        [ForeignKey("TaskPersonId")]
        [JsonIgnore]
        public virtual PsTaskPerson? PsTaskPerson { get; set; }
        public string? UserName { get; set; }
        public string? Task { get; set; }
        public string? Note { get; set; }
    }
}
