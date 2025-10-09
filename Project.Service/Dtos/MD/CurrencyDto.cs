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
    public class CurrencyDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MdCurrency, CurrencyDto>().ReverseMap();
        }
    }
}
