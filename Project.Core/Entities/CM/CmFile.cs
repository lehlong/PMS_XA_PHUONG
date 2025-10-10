using Project.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.CM
{
    public class CmFile : SoftDeleteEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public decimal? FileSize { get; set; }
        public string? MimeType { get; set; }
        public string? Extention { get; set; }
        public string? Icon { get; set; }
        public string? RefrenceFileId { get; set; }
    }
}
