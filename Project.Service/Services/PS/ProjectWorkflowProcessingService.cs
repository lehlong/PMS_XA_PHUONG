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
        Task<List<ProjectWorkflowProcessingDto>> GetProjectWorkflowStep(string projectId,string code);
        Task StartWorkflow(string projectId, string code);
        Task StartTaskWorkflow(string projectId, string code);
        Task UpdateWorkflowProject(List<ProjectWorkflowProcessingDto> request);
    }

    public class ProjectWorkflowProcessingService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProjectWorkflowProcessing, ProjectWorkflowProcessingDto>(dbContext, mapper), IProjectWorkflowProcessingService
    {
        public async Task<List<ProjectWorkflowProcessingDto>> GetProjectWorkflowStep(string projectId,string code)
        {
            try
            {
                var entities = await _dbContext.PsProjectWorkflowProcessing.Include(x => x.Person)
                                .Where(x => x.ProjectId == projectId && x.Code == code).OrderBy(x => x.Step)
                                .ToListAsync();
                var data = _mapper.Map<List<ProjectWorkflowProcessingDto>>(entities);
                foreach(var i in data)
                {
                    i.ListActions = i.Action?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectWorkflowProcessingDto>();
            }
        }

        public async Task StartWorkflow(string projectId, string code)
        {
            try
            {
                var project = await _dbContext.PsProject.FirstOrDefaultAsync(x => x.Id == projectId);
                var step = await _dbContext.PsProjectWorkflowProcessing.Where(x => x.ProjectId == projectId && x.Code == code).OrderBy(x => x.Step).FirstOrDefaultAsync();

                project.CurrentStepWorkflowId = step.Id;
                step.IsProcessing = true;
                step.Deadline = DateTime.Now.AddDays(step.HanXuLy ?? 0);

                _dbContext.PsProject.Update(project);
                _dbContext.PsProjectWorkflowProcessing.Update(step);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }
        public async Task StartTaskWorkflow(string projectId,string code)
        {
            try
            {
                var projectstruct = await _dbContext.PsProjectStruct.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Code == code);
                var step = await _dbContext.PsProjectWorkflowProcessing.Where(x => x.ProjectId == projectId && x.Code == code).OrderBy(x => x.Step).FirstOrDefaultAsync();

                projectstruct.CurrentStepWorkflowId = step.Id;
                step.IsProcessing = true;
                step.Deadline = DateTime.Now.AddDays(step.HanXuLy ?? 0);

                _dbContext.PsProjectStruct.Update(projectstruct);
                _dbContext.PsProjectWorkflowProcessing.Update(step);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task UpdateWorkflowProject(List<ProjectWorkflowProcessingDto> request)
        {
            try
            {
                foreach(var i in request)
                {
                    i.Action = string.Join(",", i.ListActions);
                }
                var entities = _mapper.Map<List<PsProjectWorkflowProcessing>>(request);
                _dbContext.PsProjectWorkflowProcessing.UpdateRange(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }
    }
}
