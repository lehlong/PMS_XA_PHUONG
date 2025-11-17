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
        Task<List<ProjectPersonDto>> GetProjectPersonByOrg(string projectId, string orgId = null);
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

        public async Task<List<ProjectPersonDto>> GetProjectPersonByOrg(string projectId, string orgId = null)
        {
            try
            {
                // Bắt đầu query cơ bản, giống hàm GetProjectPerson
                var query = _dbContext.PsProjectPerson
                                    .Include(x => x.Person)
                                    .ThenInclude(x => x.Title)
                                    .Where(x => x.ProjectId == projectId);
                if (!string.IsNullOrEmpty(orgId) && orgId != "G00")
                {
                    query = query.Where(x => x.Person.OrgId == orgId);
                }
                var result = await query.Select(p => new ProjectPersonDto
                {
                    Id = p.Id,
                    ProjectId = p.ProjectId,
                    UserName = p.UserName,
                    ProjectRoleCode = p.ProjectRoleCode,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Person = p.Person,
                    TaskCount = _dbContext.PsTaskPerson
                                .Where(t => t.UserName == p.UserName)
                                .SelectMany(t => t.TaskPersonDetails) // Vào bảng Detail
                                .Count()
                }).ToListAsync();

                return result;
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
