using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.AD
{
    public class AccountDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? OrgId { get; set; }
        public string? TitleCode { get; set; }
        public List<AdAccountGroup>? AccountGroups { get; set; }
        public List<string>? Rights { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdAccount, AccountDto>().ReverseMap();
        }
    }
}
