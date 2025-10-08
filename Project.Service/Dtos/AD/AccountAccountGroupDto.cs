using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Dtos.AD
{
    public class AccountAccountGroupDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; } 
        public string? UserName { get; set; }
        public string? GroupId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdAccountAccountGroup, AccountAccountGroupDto>().ReverseMap();
        }
    }
}
