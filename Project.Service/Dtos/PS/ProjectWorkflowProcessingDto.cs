using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using Project.Core.Entities.PS;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.PS
{
    public class ProjectWorkflowProcessingDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? ProjectId { get; set; }
        public string? WorkflowId { get; set; }
        public string? NextId { get; set; } 
        public int? Step { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public int? HanXuLy { get; set; }
        public string? Action { get; set; }
        public bool? IsDone { get; set; }
        public bool? IsProcessing { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int? Acted { get; set; }
        public DateTime? UpdateDate { get; set; }
        public virtual AdAccount? Person { get; set; }
        public List<int>? ListActions { get; set; } 

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PsProjectWorkflowProcessing, ProjectWorkflowProcessingDto>();

            profile.CreateMap<ProjectWorkflowProcessingDto, PsProjectWorkflowProcessing>()
                .ForMember(dest => dest.Person, opt => opt.Ignore());
        }
    }
}
