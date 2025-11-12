using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.PS;
using Project.Service.Common;
using Project.Service.Dtos.PS;

namespace Project.Service.Services.PS
{
    public interface IProjectPersonService : IGenericService<PsProjectPerson, ProjectPersonDto>
    {
        Task<List<ProjectPersonDto>> GetProjectPerson(string projectId);
        Task<List<ProjectPersonDto>> GetProjectPersonByOrg(string projectId, string orgId);
        Task UpdateProjectPerson(List<ProjectPersonDto> request);
        Task UpdateInfoProjectPerson(List<ProjectPersonDto> request);
        Task DeleteProjectPerson(List<string> ids);
    }

    public class ProjectPersonService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProjectPerson, ProjectPersonDto>(dbContext, mapper), IProjectPersonService
    {
        public async Task<List<ProjectPersonDto>> GetProjectPerson(string projectId)
        {
            try
            {
                var entities = await _dbContext.PsProjectPerson.Include(x => x.Person).ThenInclude(x => x.Title)
                                .Where(x => x.ProjectId == projectId)
                                .ToListAsync();
                return _mapper.Map<List<ProjectPersonDto>>(entities);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectPersonDto>();
            }

        }

        public async Task<List<ProjectPersonDto>> GetProjectPersonByOrg(string projectId, string orgId)
        {
            try
            {
                // Bắt đầu query cơ bản, giống hàm GetProjectPerson
                var query = _dbContext.PsProjectPerson
                                    .Include(x => x.Person)
                                    .ThenInclude(x => x.Title)
                                    .Where(x => x.ProjectId == projectId);
                // Nếu orgId là "G00" hoặc null/empty, nó sẽ bỏ qua bộ lọc này
                // và trả về tất cả nhân sự của dự án.
                if (!string.IsNullOrEmpty(orgId) && orgId != "G00")
                {
                    // Thêm điều kiện lọc theo OrgId trong đối tượng Person liên quan
                    query = query.Where(x => x.Person.OrgId == orgId);
                }

                var entities = await query.ToListAsync();
                return _mapper.Map<List<ProjectPersonDto>>(entities);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectPersonDto>();
            }
        }

        public async Task UpdateProjectPerson(List<ProjectPersonDto> request)
        {
            try
            {
                foreach (var i in request)
                {
                    i.Id = Guid.NewGuid().ToString();
                    await _dbContext.PsProjectPerson.AddAsync(_mapper.Map<PsProjectPerson>(i));
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task UpdateInfoProjectPerson(List<ProjectPersonDto> request)
        {
            try
            {
                var entities = _mapper.Map<List<PsProjectPerson>>(request);
                _dbContext.PsProjectPerson.UpdateRange(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task DeleteProjectPerson(List<string> ids)
        {
            try
            {
                await _dbContext.PsProjectPerson.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
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
