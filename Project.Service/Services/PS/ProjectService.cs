using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Common;
using Project.Core.Entities.CM;
using Project.Core.Entities.PS;
using Project.Core.Statics;
using Project.Service.Common;
using Project.Service.Dtos.CM;
using Project.Service.Dtos.MD;
using Project.Service.Dtos.PS;

namespace Project.Service.Services.PS
{
    public interface IProjectService : IGenericService<PsProject, ProjectDto>
    {
        Task<string> CreateProject(ProjectDto request);
        Task<ProjectDto> GetProjectDetail(string projectId);
        Task<List<ProjectStructDto>> GetGiaiDoan(string projectId);
        Task<ProjectWorkflowProcessingDto> GetCurrentStep(string stepId);
        Task<List<ProjectDto>> GetProjectWorkflow(string projectId);
        Task TrinhDuyet(string projectId);
        Task XacNhan(string projectId);
        Task PheDuyet(string projectId);
        Task YeuCauChinhSua(string projectId);
        Task TuChoi(string projectId);
    }

    public class ProjectService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProject, ProjectDto>(dbContext, mapper), IProjectService
    {
        public override async Task<PagedResponseDto> Search(ProjectDto filter)
        {
            try
            {
                var query = _dbContext.PsProject.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.Code.Contains(filter.KeyWord) || x.Name.Contains(filter.KeyWord));
                }

                if (!string.IsNullOrWhiteSpace(filter.DonViPhuTrach))
                {
                    query = query.Where(x => x.DonViPhuTrach == filter.DonViPhuTrach);
                }

                if (!string.IsNullOrWhiteSpace(filter.LanhDaoPhuTrach))
                {
                    query = query.Where(x => x.LanhDaoPhuTrach == filter.LanhDaoPhuTrach);
                }

                if (!string.IsNullOrWhiteSpace(filter.LoaiDuAn))
                {
                    query = query.Where(x => x.LoaiDuAn == filter.LoaiDuAn);
                }

                if (!string.IsNullOrWhiteSpace(filter.PmDuAn))
                {
                    query = query.Where(x => x.PmDuAn == filter.PmDuAn);
                }

                if (!string.IsNullOrWhiteSpace(filter.PmDuAn))
                {
                    query = query.Where(x => x.PmDuAn == filter.PmDuAn);
                }

                if (!string.IsNullOrWhiteSpace(filter.CapDuAn))
                {
                    query = query.Where(x => x.CapDuAn == filter.CapDuAn);
                }

                if (!string.IsNullOrWhiteSpace(filter.PmDuAn))
                {
                    query = query.Where(x => x.PmDuAn == filter.PmDuAn);
                }

                if (!string.IsNullOrWhiteSpace(filter.KhuVuc))
                {
                    query = query.Where(x => x.KhuVuc == filter.KhuVuc);
                }

                query = query.OrderByDescending(x => x.CreateDate);
                return await Paging(query, filter);

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return new PagedResponseDto();
            }
        }

        public async Task<ProjectWorkflowProcessingDto> GetCurrentStep(string stepId)
        {
            try
            {
                var _step = await _dbContext.PsProjectWorkflowProcessing.FirstOrDefaultAsync(x => x.Id == stepId);
                var step = _mapper.Map<ProjectWorkflowProcessingDto>(_step);
                step.ListActions = step.Action?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                return step;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return new ProjectWorkflowProcessingDto();
            }
        }
        public async Task<List<ProjectDto>> GetProjectWorkflow(string projectId)
        {
            try
            {

                var _structs = await _dbContext.PsProject.Where(x => x.Id == projectId && x.WorkflowId != null).Include(x => x.Workflow).ToListAsync();
                return _mapper.Map<List<ProjectDto>>(_structs);
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectDto>();
            }
        }

        public async Task<string> CreateProject(ProjectDto request)
        {
            try
            {
                #region Validate

                #endregion

                #region Xử lý thông tin dự án
                var projectId = Guid.NewGuid().ToString();
                var project = _mapper.Map<PsProject>(request);
                project.Id = projectId;
                project.GiaiDoan = 0;
                project.RefrenceFileId = Guid.NewGuid().ToString();

                await _dbContext.PsProject.AddAsync(project);
                #endregion

                #region Xử lý cây cấu trúc
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
                    WorkflowId = request.WorkflowId,
                    Code = request.Code,
                    Name = request.Name,
                    PId = "STRUCT_PROJECT",
                    OrderNumber = 0,
                    Expanded = true,
                    OrgId = request.DonViPhuTrach,
                    Type = ProjectStructType.Project,
                    RefrenceFileId = Guid.NewGuid().ToString()
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
                        RefrenceFileId = Guid.NewGuid().ToString()
                    });

                    idMapping[i.Id] = newId;
                }
                #endregion

                #region Xử lý file đính kèm
                var lstFile = _mapper.Map<List<CmFile>>(request.Files);

                foreach (var f in lstFile)
                {
                    f.RefrenceFileId = project.RefrenceFileId;
                }
                #endregion

                #region Xử lý workflow
                var lstStepConfig = await _dbContext.MdWorkflowStep.Where(x => x.WorkflowId == request.WorkflowId).OrderBy(x => x.Step).ToListAsync();

                var lstProcessing = new List<PsProjectWorkflowProcessing>();

                foreach (var i in lstStepConfig)
                {
                    lstProcessing.Add(new PsProjectWorkflowProcessing
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = request.Code,
                        ProjectId = projectId,
                        WorkflowId = request.WorkflowId,
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
                    {
                        lstProcessing[idx].NextId = lstProcessing[idx + 1].Id;
                    }
                    else
                    {
                        lstProcessing[idx].NextId = null;
                    }
                    if (idx > 0)
                    {
                        // Nếu không phải phần tử đầu tiên, lấy ID của phần tử đứng trước nó
                        lstProcessing[idx].PreviousId = lstProcessing[idx - 1].Id;
                    }
                    else
                    {
                        // Nếu là phần tử đầu tiên (idx == 0), không có bước trước đó
                        lstProcessing[idx].PreviousId = null;
                    }
                }

                await _dbContext.PsProjectWorkflowProcessing.AddRangeAsync(lstProcessing);

                #endregion

                #region Lưu lịch sử

                #endregion

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
                    .FirstOrDefaultAsync(x => x.Id == projectId);

                if (_project == null)
                {
                    this.MessageObject.Message = "Dự án không tồn tại trên hệ thống!";
                    this.Status = false;
                    return new ProjectDto();
                }
                return _mapper.Map<ProjectDto>(_project); ;
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new ProjectDto();
            }
        }

        public async Task<List<ProjectStructDto>> GetGiaiDoan(string projectId)
        {
            try
            {
                var _struct = await _dbContext.PsProjectStruct.Where(x => x.ProjectId == projectId && x.Type == ProjectStructType.GiaiDoan).OrderBy(x => x.OrderNumber).ToListAsync();
                return _mapper.Map<List<ProjectStructDto>>(_struct); ;
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return new List<ProjectStructDto>();
            }
        }

        public async Task TrinhDuyet(string projectId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProject.FirstOrDefaultAsync(x => x.Id == projectId);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {projectId}";
                    return;
                }

                var currentStep = await _dbContext.PsProjectWorkflowProcessing
                    .FirstOrDefaultAsync(x => x.Id == project.CurrentStepWorkflowId);
                if (currentStep == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý hiện tại của quy trình workflow!";
                    return;
                }

                project.TrangThai = ProjectStatus.DaTrinhDuyet;

                currentStep.IsDone = true;
                currentStep.IsProcessing = false;
                currentStep.Acted = WorkflowProjectAction.TrinhDuyet;

                if (!string.IsNullOrEmpty(currentStep.NextId))
                {
                    project.CurrentStepWorkflowId = currentStep.NextId;

                    var nextStep = await _dbContext.PsProjectWorkflowProcessing
                        .FirstOrDefaultAsync(x => x.Id == currentStep.NextId);

                    if (nextStep != null)
                    {
                        nextStep.IsDone = false;
                        nextStep.IsProcessing = true;
                        nextStep.Deadline = DateTime.Now.AddDays(nextStep.HanXuLy ?? 0);

                        _dbContext.PsProjectWorkflowProcessing.Update(nextStep);
                    }
                    else
                    {
                        this.Status = false;
                        this.MessageObject.Message = "Lỗi hệ thống!";
                        this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý tiếp theo của quy trình workflow!";
                        return;
                    }
                }
                else
                {
                    project.CurrentStepWorkflowId = null; 
                }

                _dbContext.PsProject.Update(project);
                _dbContext.PsProjectWorkflowProcessing.Update(currentStep);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task XacNhan(string projectId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProject.FirstOrDefaultAsync(x => x.Id == projectId);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {projectId}";
                    return;
                }

                var currentStep = await _dbContext.PsProjectWorkflowProcessing
                    .FirstOrDefaultAsync(x => x.Id == project.CurrentStepWorkflowId);
                if (currentStep == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý hiện tại của quy trình workflow!";
                    return;
                }

                project.TrangThai = ProjectStatus.DaXacNhan;

                currentStep.IsDone = true;
                currentStep.IsProcessing = false;
                currentStep.Acted = WorkflowProjectAction.XacNhan;

                if (!string.IsNullOrEmpty(currentStep.NextId))
                {
                    project.CurrentStepWorkflowId = currentStep.NextId;

                    var nextStep = await _dbContext.PsProjectWorkflowProcessing
                        .FirstOrDefaultAsync(x => x.Id == currentStep.NextId);

                    if (nextStep != null)
                    {
                        nextStep.IsDone = false;
                        nextStep.IsProcessing = true;
                        nextStep.Deadline = DateTime.Now.AddDays(nextStep.HanXuLy ?? 0);

                        _dbContext.PsProjectWorkflowProcessing.Update(nextStep);
                    }
                    else
                    {
                        this.Status = false;
                        this.MessageObject.Message = "Lỗi hệ thống!";
                        this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý tiếp theo của quy trình workflow!";
                        return;
                    }
                }
                else
                {
                    project.CurrentStepWorkflowId = null;
                }

                _dbContext.PsProject.Update(project);
                _dbContext.PsProjectWorkflowProcessing.Update(currentStep);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task PheDuyet(string projectId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProject.FirstOrDefaultAsync(x => x.Id == projectId);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {projectId}";
                    return;
                }

                var currentStep = await _dbContext.PsProjectWorkflowProcessing
                    .FirstOrDefaultAsync(x => x.Id == project.CurrentStepWorkflowId);
                if (currentStep == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý hiện tại của quy trình workflow!";
                    return;
                }

                project.TrangThai = ProjectStatus.DaPheDuyet;

                currentStep.IsDone = true;
                currentStep.IsProcessing = false;
                currentStep.Acted = WorkflowProjectAction.PheDuyet;

                if (!string.IsNullOrEmpty(currentStep.NextId))
                {
                    project.CurrentStepWorkflowId = currentStep.NextId;

                    var nextStep = await _dbContext.PsProjectWorkflowProcessing
                        .FirstOrDefaultAsync(x => x.Id == currentStep.NextId);

                    if (nextStep != null)
                    {
                        nextStep.IsDone = false;
                        nextStep.IsProcessing = true;
                        nextStep.Deadline = DateTime.Now.AddDays(nextStep.HanXuLy ?? 0);

                        _dbContext.PsProjectWorkflowProcessing.Update(nextStep);
                    }
                    else
                    {
                        this.Status = false;
                        this.MessageObject.Message = "Lỗi hệ thống!";
                        this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý tiếp theo của quy trình workflow!";
                        return;
                    }
                }
                else
                {
                    project.CurrentStepWorkflowId = null;
                }

                _dbContext.PsProject.Update(project);
                _dbContext.PsProjectWorkflowProcessing.Update(currentStep);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task TuChoi(string projectId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProject.FirstOrDefaultAsync(x => x.Id == projectId);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {projectId}";
                    return;
                }

                var currentStep = await _dbContext.PsProjectWorkflowProcessing
                    .FirstOrDefaultAsync(x => x.Id == project.CurrentStepWorkflowId);
                if (currentStep == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý hiện tại của quy trình workflow!";
                    return;
                }

                project.TrangThai = ProjectStatus.TuChoi;

                currentStep.IsDone = true;
                currentStep.IsProcessing = false;
                currentStep.Acted = WorkflowProjectAction.TuChoi;

                if (!string.IsNullOrEmpty(currentStep.NextId))
                {
                    project.CurrentStepWorkflowId = currentStep.NextId;

                    var nextStep = await _dbContext.PsProjectWorkflowProcessing
                        .FirstOrDefaultAsync(x => x.Id == currentStep.NextId);

                    if (nextStep != null)
                    {
                        nextStep.IsDone = false;
                        nextStep.IsProcessing = true;
                        nextStep.Deadline = DateTime.Now.AddDays(nextStep.HanXuLy ?? 0);

                        _dbContext.PsProjectWorkflowProcessing.Update(nextStep);
                    }
                    else
                    {
                        this.Status = false;
                        this.MessageObject.Message = "Lỗi hệ thống!";
                        this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý tiếp theo của quy trình workflow!";
                        return;
                    }
                }
                else
                {
                    project.CurrentStepWorkflowId = null;
                }

                _dbContext.PsProject.Update(project);
                _dbContext.PsProjectWorkflowProcessing.Update(currentStep);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task YeuCauChinhSua(string projectId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProject.FirstOrDefaultAsync(x => x.Id == projectId);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {projectId}";
                    return;
                }

                var currentStep = await _dbContext.PsProjectWorkflowProcessing
                    .FirstOrDefaultAsync(x => x.Id == project.CurrentStepWorkflowId);
                if (currentStep == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý hiện tại của quy trình workflow!";
                    return;
                }

                project.TrangThai = ProjectStatus.YeuCauChinhSua;

                currentStep.IsDone = false;
                currentStep.IsProcessing = false;
                currentStep.Acted = WorkflowProjectAction.YeuCauChinhSua;

                if (!string.IsNullOrEmpty(currentStep.PreviousId))
                {
                    project.CurrentStepWorkflowId = currentStep.PreviousId;

                    var previousStep = await _dbContext.PsProjectWorkflowProcessing
                        .FirstOrDefaultAsync(x => x.Id == currentStep.PreviousId);

                    if (previousStep != null)
                    {
                        previousStep.IsDone = false;
                        previousStep.IsProcessing = true;
                        previousStep.Deadline = DateTime.Now.AddDays(previousStep.HanXuLy ?? 0);

                        _dbContext.PsProjectWorkflowProcessing.Update(previousStep);
                    }
                    else
                    {
                        this.Status = false;
                        this.MessageObject.Message = "Lỗi hệ thống!";
                        this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý tiếp theo của quy trình workflow!";
                        return;
                    }
                }
                else
                {
                    project.CurrentStepWorkflowId = null;
                }

                _dbContext.PsProject.Update(project);
                _dbContext.PsProjectWorkflowProcessing.Update(currentStep);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                this.Status = false;
                this.Exception = ex;
            }
        }

    }
}
