using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.MD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.MD
{
    public class WorkflowDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ProjectLevelCode { get; set; }
        public string? OrgId { get; set; }
        public int? Type { get; set; }
        public string? Notes { get; set; }
        public virtual ICollection<WorkflowStepDto>? Steps { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<MdWorkflow, WorkflowDto>();

            profile.CreateMap<WorkflowDto, MdWorkflow>()
                .ForMember(dest => dest.Steps, opt => opt.Ignore());
        }
    }
}
