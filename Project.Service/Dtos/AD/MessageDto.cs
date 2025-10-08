using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.AD
{
    public class MessageDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Code { get; set; }
        public string? Lang { get; set; }
        public string? Message { get; set; }
        public string? Type { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdMessage, MessageDto>().ReverseMap();
        }
    }
}
