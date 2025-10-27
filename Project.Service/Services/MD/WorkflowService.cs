using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface IWorkflowService : IGenericService<MdWorkflow, WorkflowDto>
    {
        Task InsertWorkflow(WorkflowDto request);
        Task UpdateWorkflow(WorkflowDto request);
        Task<WorkflowDto> GetDetail(string workflowId);
    }

    public class WorkflowService(AppDbContext dbContext, IMapper mapper) : GenericService<MdWorkflow, WorkflowDto>(dbContext, mapper), IWorkflowService
    {
        public override async Task<PagedResponseDto> Search(WorkflowDto filter)
        {
            try
            {
                var query = _dbContext.MdWorkflow.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.OrgId))
                {
                    query = query.Where(x => x.OrgId == filter.OrgId);
                }

                return await Paging(query, filter);

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return new PagedResponseDto();
            }
        }

        public async Task InsertWorkflow(WorkflowDto request)
        {
            try
            {
                var workflowId = Guid.NewGuid().ToString();
                var _workflow = _mapper.Map<MdWorkflow>(request);
                _workflow.Id = workflowId;
                await _dbContext.MdWorkflow.AddAsync(_workflow);

                foreach (var i in request.Steps)
                {
                    i.Id = Guid.NewGuid().ToString();
                    i.WorkflowId = workflowId;
                    i.Action = string.Join(",", i.ListActions);
                }

                var _steps = _mapper.Map<List<MdWorkflowStep>>(request.Steps);
                await _dbContext.MdWorkflowStep.AddRangeAsync(_steps);

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task UpdateWorkflow(WorkflowDto request)
        {
            try
            {
                var _workflow = _mapper.Map<MdWorkflow>(request);
                _dbContext.MdWorkflow.Update(_workflow);

                foreach(var i in request.Steps)
                {
                    i.Action = string.Join(",", i.ListActions);
                }

                var _steps = _mapper.Map<List<MdWorkflowStep>>(request.Steps);
                _dbContext.MdWorkflowStep.UpdateRange(_steps);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task<WorkflowDto> GetDetail(string workflowId)
        {
            try
            {
                var _workflow = await _dbContext.MdWorkflow.Include(x => x.Steps).FirstOrDefaultAsync(x => x.Id == workflowId);
                if (_workflow.Steps != null)
                {
                    _workflow.Steps = _workflow.Steps.OrderBy(x => x.Step).ToList();
                }

                var data = _mapper.Map<WorkflowDto>(_workflow);

                foreach (var i in data.Steps)
                {
                    i.ListActions = i.Action?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return new WorkflowDto();
            }
        }
    }
}
