﻿using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.AD;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.AD
{
    public class HistoryLoginDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; } 
        public string? UserName { get; set; }
        public DateTime? LogonTime { get; set; }
        public string? Browser { get; set; }
        public string? Version { get; set; }
        public bool? IsMobile { get; set; }
        public string? Os { get; set; }
        public string? MobileModel { get; set; }
        public string? Manufacturer { get; set; }
        public string? IpAddress { get; set; }
        public bool? Status { get; set; }
        public string? Reason { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdHistoryLogin, HistoryLoginDto>().ReverseMap();
        }
    }
}
