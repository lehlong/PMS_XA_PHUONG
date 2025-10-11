using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PsProjectPerson, ProjectPersonDto>().ReverseMap();
        }
    }
}
