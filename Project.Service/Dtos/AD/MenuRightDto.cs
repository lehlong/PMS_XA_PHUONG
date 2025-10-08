using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.AD
{
    public class MenuRightDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? MenuId { get; set; }
        public string? RightId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdMenuRight, MenuRightDto>().ReverseMap();
        }
    }
}
