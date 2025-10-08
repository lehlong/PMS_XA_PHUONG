using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.MD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.MD
{
    public class OrganizeDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; } 
        public string? Name { get; set; }
        public string? PId { get; set; }
        public int? OrderNumber { get; set; }
        public bool? Expanded { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MdOrganize, OrganizeDto>().ReverseMap();
        }
    }
}
