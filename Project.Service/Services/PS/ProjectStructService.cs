using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.PS;
using Project.Service.Common;
using Project.Service.Dtos.PS;

namespace Project.Service.Services.PS
{
    public interface IProjectStructService : IGenericService<PsProjectStruct, ProjectStructDto>
    {
        Task<List<ProjectStructDto>> GetProjectStruct(string projectId);
    }

    public class ProjectStructService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProjectStruct, ProjectStructDto>(dbContext, mapper), IProjectStructService
    {
        public async Task<List<ProjectStructDto>> GetProjectStruct(string projectId)
        {
            try
            {
                var _structs = await _dbContext.PsProjectStruct.Where(x => x.ProjectId == projectId).OrderBy(x => x.OrderNumber).ToListAsync();
                return _mapper.Map<List<ProjectStructDto>>(_structs);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectStructDto>();
            }
        }
    }
}
