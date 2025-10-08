using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.AD
{
    public class RightDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? PId { get; set; }
        public int? OrderNumber { get; set; }
        public bool? Expanded { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdRight, RightDto>().ReverseMap();
        }
    }
}
