using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.CM;
using Project.Core.Entities.MD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Dtos.CM
{
    public class FileDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public decimal? FileSize { get; set; }
        public string? MimeType { get; set; }
        public string? Extention { get; set; }
        public string? Icon { get; set; }
        public string? RefrenceFileId { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CmFile, FileDto>().ReverseMap();
        }
    }
}
