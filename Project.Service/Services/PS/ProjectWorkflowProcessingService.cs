using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.PS;
using Project.Service.Common;
using Project.Service.Dtos.PS;

namespace Project.Service.Services.PS
{
    public interface IProjectWorkflowProcessingService : IGenericService<PsProjectWorkflowProcessing, ProjectWorkflowProcessingDto>
    {
        Task<List<ProjectWorkflowProcessingDto>> GetProjectWorkflowStep(string projectId);
    }

    public class ProjectWorkflowProcessingService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProjectWorkflowProcessing, ProjectWorkflowProcessingDto>(dbContext, mapper), IProjectWorkflowProcessingService
    {
        public async Task<List<ProjectWorkflowProcessingDto>> GetProjectWorkflowStep(string projectId)
        {
            try
            {
                var entities = await _dbContext.PsProjectWorkflowProcessing
                                .Where(x => x.ProjectId == projectId).OrderBy(x => x.Step)
                                .ToListAsync();
                return _mapper.Map<List<ProjectWorkflowProcessingDto>>(entities);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectWorkflowProcessingDto>();
            }
        }
    }
}
