using Project.Core.Common;
using Project.Core.Entities.CM;
using Project.Core.Entities.MD;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.PS
{
    public class PsProject : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
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
        public string? RefrenceFileId { get; set; }
        public virtual MdOrganize? DonViPhuTrachRef { get; set; }
        public virtual ICollection<CmFile>? Files { get; set; }
        public virtual ICollection<PsProjectStruct>? Structs { get; set; }

    }
}
