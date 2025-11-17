using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.CM;
using Project.Core.Entities.MD;
using Project.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Dtos.PS
{
    public class ProjectStructDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; } 
        public string? ProjectId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? PId { get; set; }
        public string? WorkflowId { get; set; }
        public int? OrderNumber { get; set; }
        public bool? Expanded { get; set; }
        public string? OrgId { get; set; }
        public int? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? RefrenceFileId { get; set; }
        public string? Notes { get; set; }
        public PsProject? Project { get; set; }

        public MdWorkflow? Workflow { get; set; }
        public ICollection<CmFile>? Files { get; set; }
        public ICollection<PsTaskPerson>? TaskPerson { get; set; }

        public void Mapping(Profile profile)
        {

            profile.CreateMap<PsProjectStruct, ProjectStructDto>();

            profile.CreateMap<ProjectStructDto, PsProjectStruct>()
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.Workflow, opt => opt.Ignore())
                .ForMember(dest => dest.TaskPerson, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore());
        }
    }
    public class TaskPersonDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? TaskId { get; set; }
        public string? ProjectId { get; set; }
        public string? UserName { get; set; }
        public List<int>? TaskRoles { get; set; }
        public string? ProjectRoleCode { get; set; }
        public ICollection<PsTaskPersonDetail>? TaskPersonDetails { get; set; }
        public void Mapping(Profile profile)
        {

            profile.CreateMap<PsTaskPerson, TaskPersonDto>();

            profile.CreateMap<TaskPersonDto, PsTaskPerson>()
                .ForMember(dest => dest.TaskPersonDetails, opt => opt.Ignore());
                
        }
    }
    public class TaskPersonDetailDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? TaskPersonId { get; set; }
        public string? UserName { get; set; }
        public string? Task { get; set; }
        public string? Note { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<PsTaskPersonDetail, TaskPersonDetailDto>();
        }
    }
}
