using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.AD
{
    public class MenuDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? PId { get; set; }
        public int? OrderNumber { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public bool? Expanded { get; set; }
        public List<MenuRightDto>? MenuRights { get; set; } = new List<MenuRightDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdMenu, MenuDto>().ReverseMap();
        }
    }
}
