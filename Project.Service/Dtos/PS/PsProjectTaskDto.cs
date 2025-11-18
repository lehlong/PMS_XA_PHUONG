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
    public class ProjectTaskDto : BaseDto, IMapFrom, IDto
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

        public void Mapping(Profile profile)
        {

            profile.CreateMap<PsProjectStruct, ProjectTaskDto>();
            profile.CreateMap<ProjectTaskDto, PsProjectStruct>();
        }
    }
}