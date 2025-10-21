using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using Project.Core.Entities.PS;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.PS
{
    public class ProjectPersonDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; } 
        public string? ProjectId { get; set; }
        public string? UserName { get; set; }
        public string? ProjectRoleCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AdAccount? Person { get; set; }
        public PsProject? Project { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PsProjectPerson, ProjectPersonDto>();

            profile.CreateMap<ProjectPersonDto, PsProjectPerson>()
                .ForMember(dest => dest.Person, opt => opt.Ignore());
        }
    }
}
