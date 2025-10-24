using AutoMapper;
using Project.Core.Common;
using Project.Core.Entities.PS;
using System.ComponentModel.DataAnnotations;

namespace Project.Service.Dtos.PS
{
    public class ProjectWorkflowProcessingDto : BaseDto, IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public int? Step { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public int? HanXuLy { get; set; }
        public int? Action { get; set; }
        public bool? IsDone { get; set; }
        public bool? IsProcessing { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletionDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PsProjectWorkflowProcessing, ProjectWorkflowProcessingDto>();

            profile.CreateMap<ProjectWorkflowProcessingDto, PsProjectWorkflowProcessing>();
        }
    }
}
