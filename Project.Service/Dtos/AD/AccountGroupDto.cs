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
    public class AccountGroupDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? OrgId { get; set; }
        public string? Name { get; set; } 
        public string? Notes { get; set; }
        public List<AccountGroupRightDto>? AccountGroupRights { get; set; } = new List<AccountGroupRightDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdAccountGroup, AccountGroupDto>().ReverseMap();
        }
    }
}
