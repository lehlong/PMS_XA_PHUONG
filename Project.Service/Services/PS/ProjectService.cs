using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.CM;
using Project.Core.Entities.PS;
using Project.Core.Statics;
using Project.Service.Common;
using Project.Service.Dtos.CM;
using Project.Service.Dtos.PS;

namespace Project.Service.Services.PS
{
    public interface IProjectService : IGenericService<PsProject, ProjectDto>
    {
        Task<string> CreateProject(ProjectDto request);
        Task<ProjectDto> GetProjectDetail(string projectId);
    }

    public class ProjectService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProject, ProjectDto>(dbContext, mapper), IProjectService
    {
        public async Task<string> CreateProject(ProjectDto request)
        {
            try
            {
                var projectId = Guid.NewGuid().ToString();
                var project = _mapper.Map<PsProject>(request);
                project.Id = projectId;
                project.GiaiDoan = 0;
                project.RefrenceFileId = Guid.NewGuid().ToString();

                await _dbContext.PsProject.AddAsync(project);

                var lstConfigStruct = await _dbContext.MdConfigStruct
                    .Where(x => x.OrgId == request.DonViPhuTrach)
                    .OrderBy(x => x.OrderNumber)
                    .ToArrayAsync();

                var rootStructId = Guid.NewGuid().ToString();

                var idMapping = new Dictionary<string, string> { { "STRUCT", rootStructId } };

                await _dbContext.PsProjectStruct.AddAsync(new PsProjectStruct
                {
                    Id = rootStructId,
                    ProjectId = projectId,
                    Code = request.Code,
                    Name = request.Name,
                    PId = "STRUCT_PROJECT",
                    OrderNumber = 0,
                    Expanded = true,
                    OrgId = request.DonViPhuTrach,
                    Type = ProjectStructType.Project,
                });

                foreach (var i in lstConfigStruct)
                {
                    var newId = Guid.NewGuid().ToString();
                    var parentId = idMapping.ContainsKey(i.PId) ? idMapping[i.PId] : null;

                    await _dbContext.PsProjectStruct.AddAsync(new PsProjectStruct
                    {
                        Id = newId,
                        ProjectId = projectId,
                        Code = i.Code,
                        Name = i.Name,
                        PId = parentId,
                        OrderNumber = i.OrderNumber,
                        Expanded = true,
                        OrgId = i.OrgId,
                        Type = i.Type,
                    });

                    idMapping[i.Id] = newId;
                }

                var lstFile = _mapper.Map<List<CmFile>>(request.Files);

                foreach(var f in lstFile)
                {
                    f.RefrenceFileId = project.RefrenceFileId;
                }

                await _dbContext.CmFile.AddRangeAsync(lstFile);

                await _dbContext.SaveChangesAsync();

                return projectId;
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return string.Empty;
            }
        }


        public async Task<ProjectDto> GetProjectDetail(string projectId)
        {
            try
            {
                var _project = await _dbContext.PsProject.Include(x => x.DonViPhuTrachRef)
                    .Include(x => x.Files)
                    .Include(x => x.Structs)
                    .FirstOrDefaultAsync(x => x.Id == projectId);

                if (_project == null)
                {
                    this.MessageObject.Message = "Dự án không tồn tại trên hệ thống!";
                    this.Status = false;
                    return new ProjectDto();
                }

                var project = _mapper.Map<ProjectDto>(_project);

                project.ListGiaiDoan = project.Structs?.Where(x => x.Type == ProjectStructType.GiaiDoan).ToList();


                return project;
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new ProjectDto();
            }
        }
    }
}
