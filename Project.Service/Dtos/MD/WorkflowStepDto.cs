using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.MD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.MD
{
    public class WorkflowStepDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? WorkflowId { get; set; }
        public int? Step { get; set; }
        public string? Name { get; set; }
        public int? HanXuLy { get; set; }
        public string? Action { get; set; }
        public List<int>? ListActions { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MdWorkflowStep, WorkflowStepDto>();

            profile.CreateMap<WorkflowStepDto, MdWorkflowStep>()
                .ForMember(dest => dest.Workflow, opt => opt.Ignore()); ;
        }
    }
}
