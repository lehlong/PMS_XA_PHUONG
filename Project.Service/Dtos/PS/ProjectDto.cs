using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.MD;
using Project.Core.Entities.PS;
using Project.Service.Dtos.CM;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.PS
{
    public class ProjectDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? DonViPhuTrach { get; set; }
        public string? LanhDaoPhuTrach { get; set; }
        public string? LoaiDuAn { get; set; }
        public string? PmDuAn { get; set; }
        public string? CapDuAn { get; set; }
        public string? PhuTrachDuAn { get; set; }
        public string? QuanLyHopDong { get; set; }
        public string? KhachHang { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? KhuVuc { get; set; }
        public string? DiaDiem { get; set; }
        public int? TrangThai { get; set; }
        public int? GiaiDoan { get; set; }
        public string? Notes { get; set; }
        public string? WorkflowId { get; set; }
        public string? RefrenceFileId { get; set; }
        public ICollection<ProjectStructDto>? ListGiaiDoan { get; set; }
        public ICollection<ProjectStructDto>? Structs { get; set; }
        public ICollection<FileDto>? Files { get; set; }
        public MdOrganize? DonViPhuTrachRef { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PsProject, ProjectDto>();

            profile.CreateMap<ProjectDto, PsProject>()
                .ForMember(dest => dest.Structs, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ForMember(dest => dest.DonViPhuTrachRef, opt => opt.Ignore());
        }
    }
}