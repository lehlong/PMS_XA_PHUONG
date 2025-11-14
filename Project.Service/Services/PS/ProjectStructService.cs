using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.CM;
using Project.Core.Entities.PS;
using Project.Core.Statics;
using Project.Service.Common;
using Project.Service.Dtos.PS;

namespace Project.Service.Services.PS
{
    public interface IProjectStructService : IGenericService<PsProjectStruct, ProjectStructDto>
    {
        Task<List<ProjectStructDto>> GetProjectStruct(string projectId);
        Task<List<ProjectStructDto>> GetTaskWorkflow(string projectId);
        Task Insert(ProjectStructDto request);
    }

    public class ProjectStructService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProjectStruct, ProjectStructDto>(dbContext, mapper), IProjectStructService
    {

        //public override async Task<PagedResponseDto> Search(ProjectStructDto filter)
        //{
        //    try
        //    {
        //        var query = _dbContext.PsProjectStruct.Include(x => x.Workflow).Include(x => x.Project)   
        //        .ThenInclude(p => p.Workflow).AsQueryable();

        //        if (!string.IsNullOrWhiteSpace(filter.KeyWord))
        //        {
        //            query = query.Where(x => x.ProjectId.Contains(filter.KeyWord));
        //        }
        //        if (!string.IsNullOrWhiteSpace(filter.ProjectId))
        //        {
        //            query = query.Where(x => x.ProjectId == filter.ProjectId);
        //            query = query.Where(x => !string.IsNullOrWhiteSpace(x.WorkflowId));
        //        }

        //        if (!string.IsNullOrWhiteSpace(filter.KeyWord))
        //        {
        //            string keywordLower = filter.KeyWord.ToLower().Trim();

        //            query = query.Where(x =>
        //                x.Workflow.Name.ToLower().Contains(keywordLower) ||
        //                x.Workflow.Code.ToLower().Contains(keywordLower)
        //            );
        //        }
        //        query = query.OrderByDescending(x => x.CreateDate);
        //        return await Paging(query, filter);

        //    }
        //    catch (Exception ex)
        //    {
        //        Status = false;
        //        Exception = ex;
        //        return new PagedResponseDto();
        //    }
        //}

        public async Task<List<ProjectStructDto>> GetProjectStruct(string projectId)
        {
            try
            {
                var _structs = await _dbContext.PsProjectStruct.Where(x => x.ProjectId == projectId).Include(x => x.Workflow).OrderBy(x => x.OrderNumber).ToListAsync();
                return _mapper.Map<List<ProjectStructDto>>(_structs);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectStructDto>();
            }
        }
        public async Task<List<ProjectStructDto>> GetTaskWorkflow(string projectId)
        {
            try
            {
                
                var _structs = await _dbContext.PsProjectStruct.Where(x => x.ProjectId == projectId && x.WorkflowId != null).Include(x => x.Workflow).OrderBy(x => x.OrderNumber).ToListAsync();
                return _mapper.Map<List<ProjectStructDto>>(_structs);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectStructDto>();
            }
        }





        public async Task Insert(ProjectStructDto request)
        {
            try
            {
                request.Id = Guid.NewGuid().ToString();
                var entities = _mapper.Map<PsProjectStruct>(request);

                entities.RefrenceFileId = Guid.NewGuid().ToString();

                await _dbContext.PsProjectStruct.AddAsync(entities);

                var lstStepConfig = await _dbContext.MdWorkflowStep.Where(x => x.WorkflowId == request.WorkflowId).OrderBy(x => x.Step).ToListAsync();

                var lstProcessing = new List<PsProjectWorkflowProcessing>();

                foreach (var i in lstStepConfig)
                {
                    lstProcessing.Add(new PsProjectWorkflowProcessing
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProjectId = request.ProjectId,
                        Step = i.Step,
                        Name = i.Name,
                        HanXuLy = i.HanXuLy,
                        Action = i.Action,
                        IsDone = false,
                        IsProcessing = false
                    });
                }

                for (int idx = 0; idx < lstProcessing.Count; idx++)
                {
                    if (idx < lstProcessing.Count - 1)
                        lstProcessing[idx].NextId = lstProcessing[idx + 1].Id;
                    else
                        lstProcessing[idx].NextId = null;
                }

                await _dbContext.PsProjectWorkflowProcessing.AddRangeAsync(lstProcessing);


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
