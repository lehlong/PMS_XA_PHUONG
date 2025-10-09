using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.MD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Dtos.MD
{
    public class ConfigStructDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? PId { get; set; }
        public int? OrderNumber { get; set; }
        public bool? Expanded { get; set; }
        public string? OrgId { get; set; }
        public int? Type { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MdConfigStruct, ConfigStructDto>().ReverseMap();
        }
    }
}
