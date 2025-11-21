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
        Task<List<ProjectStructDto>> GetTask(string taskId);
        Task<object> Insert(ProjectStructDto request);
        Task InsertTaskPerson(List<TaskPersonDto> request);
        Task<object> Update(ProjectTaskDto request);
        Task UpdateTaskPerson(List<TaskPersonDto> request);
        Task<ProjectWorkflowProcessingDto> GetCurrentStep(string projectId,string code);

        Task TrinhDuyet(string code);
        Task XacNhan(string code);
        Task PheDuyet(string code);
        Task YeuCauChinhSua(string code);
        Task TuChoi(string code);
    }

    public class ProjectStructService(AppDbContext dbContext, IMapper mapper) : GenericService<PsProjectStruct, ProjectStructDto>(dbContext, mapper), IProjectStructService
    {
        public override async Task<PagedResponseDto> Search(ProjectStructDto filter)
        {
            try
            {
                // 1. Khởi tạo Query và Include dữ liệu quan hệ
                var query = _dbContext.PsProjectStruct
                            .Include(x => x.TaskPerson)              // Lấy danh sách nhân sự
                                .ThenInclude(tp => tp.TaskPersonDetails) // Lấy chi tiết
                            .AsNoTracking()
                            .AsQueryable();

                // 2. Xây dựng bộ lọc động (Dynamic Filter)
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.ProjectId))
                    {
                        query = query.Where(x => x.ProjectId == filter.ProjectId);
                    }

                    if (!string.IsNullOrEmpty(filter.Code))
                    {
                        query = query.Where(x => x.Code.ToLower().Contains(filter.Code.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        query = query.Where(x => x.Name.ToLower().Contains(filter.Name.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(filter.OrgId))
                    {
                        query = query.Where(x => x.OrgId == filter.OrgId);
                    }

                    if (!string.IsNullOrEmpty(filter.WorkflowId))
                    {
                        query = query.Where(x => x.WorkflowId == filter.WorkflowId);
                    }
                }
                int page = filter != null && filter.CurrentPage > 0 ? filter.CurrentPage : 1;
                int size = filter != null && filter.PageSize > 0 ? filter.PageSize : 10;

                int totalRecord = await query.CountAsync();
                query = query.OrderByDescending(x => x.CreateDate);

                // Lấy dữ liệu phân trang
                var entities = await query
                                    .Skip((page - 1) * size)
                                    .Take(size)
                                    .ToListAsync();
                return new PagedResponseDto
                {
                    CurrentPage = page,
                    PageSize = size,
                    TotalRecord = totalRecord,
                    TotalPage = (int)Math.Ceiling((double)totalRecord / size),
                    Data = _mapper.Map<List<ProjectStructDto>>(entities)
                };
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return null;
            }
        }

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
        public async Task<List<ProjectStructDto>> GetTask(string taskId)
        {
            try
            {
                var query = _dbContext.PsProjectStruct
                    .Include(x => x.TaskPerson)
                    .ThenInclude(tp => tp.TaskPersonDetails)
                    .Where(x => x.Id == taskId);
                var entities = await query.ToListAsync();
                return _mapper.Map<List<ProjectStructDto>>(entities);
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





        public async Task<object> Insert(ProjectStructDto request)
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
                        Code = request.Code,
                        ProjectId = request.ProjectId,
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

                var lstTaskPersonEntity = new List<PsTaskPerson>();


                await _dbContext.SaveChangesAsync();
                return new
                {
                    Id = request.Id,
                    ProjectId = request.ProjectId
                };

            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return null;
            }
        }

        public async Task InsertTaskPerson(List<TaskPersonDto> request)
        {
            try
            {
                if (request != null && request.Count > 0)
                {
                    var listEntities = new List<PsTaskPerson>();

                    foreach (var item in request)
                    {
                        var personEntity = new PsTaskPerson();
                        personEntity.Id = Guid.NewGuid().ToString();
                        personEntity.TaskId = item.TaskId;
                        personEntity.ProjectId = item.ProjectId;
                        personEntity.UserName = item.UserName;
                        if (item.TaskRoles != null && item.TaskRoles.Count > 0)
                        {
                            personEntity.TaskRoles = string.Join(",", item.TaskRoles);
                        }
                        personEntity.TaskPersonDetails = new List<PsTaskPersonDetail>();

                        if (item.TaskPersonDetails != null && item.TaskPersonDetails.Count > 0)
                        {
                            foreach (var taskItem in item.TaskPersonDetails)
                            {
                                var detailEntity = new PsTaskPersonDetail
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    TaskPersonId = personEntity.TaskId,
                                    UserName = item.UserName,
                                    Task = taskItem.Task,
                                    Note = taskItem.Note
                                };
                                personEntity.TaskPersonDetails.Add(detailEntity);
                            }
                        }
                        listEntities.Add(personEntity);
                    }
                    await _dbContext.PsTaskPerson.AddRangeAsync(listEntities);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task<object> Update(ProjectTaskDto request)
        {
            try
            {
                var entity = await _dbContext.PsProjectStruct
                                             .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null)
                {
                    this.Status = false;
                    return new { Message = "Không tìm thấy dự án" };
                }
                bool isWorkflowChanged = entity.WorkflowId != request.WorkflowId;
                _mapper.Map(request, entity);
                if (isWorkflowChanged && !string.IsNullOrEmpty(request.WorkflowId))
                {
                    var oldProcessings = await _dbContext.PsProjectWorkflowProcessing
                                                         .Where(x => x.ProjectId == entity.ProjectId)
                                                         .ToListAsync();
                    _dbContext.PsProjectWorkflowProcessing.RemoveRange(oldProcessings);
                    var lstStepConfig = await _dbContext.MdWorkflowStep
                                                        .Where(x => x.WorkflowId == request.WorkflowId)
                                                        .OrderBy(x => x.Step)
                                                        .ToListAsync();

                    var lstProcessing = new List<PsProjectWorkflowProcessing>();

                    foreach (var i in lstStepConfig)
                    {
                        lstProcessing.Add(new PsProjectWorkflowProcessing
                        {
                            Id = Guid.NewGuid().ToString(),
                            Code = request.Code,
                            ProjectId = request.ProjectId,
                            WorkflowId = request.WorkflowId,
                            Step = i.Step,
                            Name = i.Name,
                            HanXuLy = i.HanXuLy,
                            Action = i.Action,
                            IsDone = false,
                            IsProcessing = false
                        });
                    }

                    // Link NextId cho các bước
                    for (int idx = 0; idx < lstProcessing.Count; idx++)
                    {
                        if (idx < lstProcessing.Count - 1)
                            lstProcessing[idx].NextId = lstProcessing[idx + 1].Id;
                        else
                            lstProcessing[idx].NextId = null;
                    }

                    await _dbContext.PsProjectWorkflowProcessing.AddRangeAsync(lstProcessing);
                }

                // 4. Lưu thay đổi
                await _dbContext.SaveChangesAsync();

                return new { Id = entity.Id, Message = "Cập nhật thông tin thành công" };
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
                return null;
            }
        }

        public async Task UpdateTaskPerson(List<TaskPersonDto> request)
        {
            try
            {
                if (request == null || request.Count == 0) return;

                // 2. [SỬA ĐOẠN NÀY] Lấy TaskId an toàn hơn
                // Tìm item đầu tiên có TaskId khác rỗng trong cả danh sách
                string taskId = request.FirstOrDefault(x => !string.IsNullOrEmpty(x.TaskId))?.TaskId;

                // Lấy ProjectId tương tự
                string projectId = request.FirstOrDefault(x => !string.IsNullOrEmpty(x.ProjectId))?.ProjectId;

                // 3. Nếu vẫn null thì báo lỗi ngay
                if (string.IsNullOrEmpty(taskId))
                {
                    this.Status = false;
                    // Throw exception hoặc return tùy bạn
                    throw new Exception("Dữ liệu đầu vào thiếu TaskId. Vui lòng kiểm tra lại JSON gửi lên.");
                }

                // 2. Lấy dữ liệu cũ trong DB (Bao gồm cả Detail)
                var existingList = await _dbContext.PsTaskPerson
                                                    .Include(x => x.TaskPersonDetails)
                                                    .Where(x => x.TaskId == taskId)
                                                    .ToListAsync();

                // 3. XỬ LÝ XÓA NHÂN SỰ (DELETE PARENT)
                // Tìm những người có trong DB nhưng không có trong Request gửi lên -> Xóa
                var requestIds = request.Where(x => !string.IsNullOrEmpty(x.Id)).Select(x => x.Id).ToList();
                var itemsToDelete = existingList.Where(x => !requestIds.Contains(x.Id)).ToList();

                if (itemsToDelete.Any())
                {
                    _dbContext.PsTaskPerson.RemoveRange(itemsToDelete);
                }

                // 4. DUYỆT REQUEST ĐỂ THÊM HOẶC SỬA
                foreach (var itemDto in request)
                {
                    // Đồng bộ TaskId và ProjectId cho chắc chắn
                    itemDto.TaskId = taskId;
                    itemDto.ProjectId = projectId;

                    var existingItem = existingList.FirstOrDefault(x => x.Id == itemDto.Id);

                    if (existingItem == null)
                    {
                        // =================================================
                        // CASE A: INSERT (THÊM MỚI) - GIỐNG HỆT INSERT CỦA BẠN
                        // =================================================
                        var personEntity = new PsTaskPerson();
                        personEntity.Id = Guid.NewGuid().ToString();
                        personEntity.TaskId = taskId;
                        personEntity.ProjectId = projectId;
                        personEntity.UserName = itemDto.UserName;

                        // Xử lý Roles thủ công
                        if (itemDto.TaskRoles != null && itemDto.TaskRoles.Count > 0)
                        {
                            personEntity.TaskRoles = string.Join(",", itemDto.TaskRoles);
                        }

                        // Xử lý Detail thủ công
                        personEntity.TaskPersonDetails = new List<PsTaskPersonDetail>();

                        if (itemDto.TaskPersonDetails != null && itemDto.TaskPersonDetails.Count > 0)
                        {
                            foreach (var taskItem in itemDto.TaskPersonDetails)
                            {
                                var detailEntity = new PsTaskPersonDetail
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    TaskPersonId = personEntity.Id, // Gán vào ID cha mới tạo
                                    UserName = itemDto.UserName,
                                    Task = taskItem.Task,
                                    Note = taskItem.Note
                                };
                                personEntity.TaskPersonDetails.Add(detailEntity);
                            }
                        }

                        await _dbContext.PsTaskPerson.AddAsync(personEntity);
                    }
                    else
                    {
                        // =================================================
                        // CASE B: UPDATE (CẬP NHẬT) - GÁN TAY THỦ CÔNG
                        // =================================================

                        // 1. Cập nhật thông tin cha
                        existingItem.UserName = itemDto.UserName;
                        existingItem.ProjectId = projectId;

                        // Cập nhật Roles thủ công
                        if (itemDto.TaskRoles != null && itemDto.TaskRoles.Count > 0)
                        {
                            existingItem.TaskRoles = string.Join(",", itemDto.TaskRoles);
                        }
                        else
                        {
                            existingItem.TaskRoles = null; // Nếu mảng rỗng thì set null
                        }

                        // 2. Cập nhật danh sách Detail con
                        if (itemDto.TaskPersonDetails != null)
                        {
                            var existingDetails = existingItem.TaskPersonDetails.ToList();

                            // Lấy danh sách ID của detail gửi lên
                            var reqDetailIds = itemDto.TaskPersonDetails
                                                    .Where(d => !string.IsNullOrEmpty(d.Id))
                                                    .Select(d => d.Id).ToList();

                            // B2.1: Xóa những detail thừa (có trong DB mà ko có trong request)
                            var detailsToDelete = existingDetails.Where(d => !reqDetailIds.Contains(d.Id)).ToList();
                            foreach (var d in detailsToDelete)
                            {
                                _dbContext.PsTaskPersonDetail.Remove(d);
                            }

                            // B2.2: Thêm mới hoặc Sửa detail
                            foreach (var taskItem in itemDto.TaskPersonDetails)
                            {
                                var existingDetail = existingDetails.FirstOrDefault(d => d.Id == taskItem.Id);

                                if (existingDetail == null)
                                {
                                    // -- Thêm Detail Mới (Manual) --
                                    var newDetail = new PsTaskPersonDetail
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        TaskPersonId = existingItem.Id, // Link vào cha hiện tại
                                        UserName = itemDto.UserName,
                                        Task = taskItem.Task,
                                        Note = taskItem.Note
                                    };
                                    existingItem.TaskPersonDetails.Add(newDetail);
                                }
                                else
                                {
                                    // -- Sửa Detail Cũ (Manual) --
                                    existingDetail.UserName = itemDto.UserName; // Cập nhật lại username nếu cần
                                    existingDetail.Task = taskItem.Task;
                                    existingDetail.Note = taskItem.Note;
                                }
                            }
                        }
                    }
                }

                // 5. Lưu thay đổi
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.Exception = ex;
            }
        }

        public async Task<ProjectWorkflowProcessingDto> GetCurrentStep(string projectId, string code)
        {
            try
            {

                var projectStruct = await _dbContext.PsProjectStruct
                                            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Code == code);

                // Kiểm tra nếu không tìm thấy Struct hoặc Struct chưa có CurrentStepWorkflowId (dự án chưa chạy luồng)
                if (projectStruct == null || string.IsNullOrEmpty(projectStruct.CurrentStepWorkflowId))
                {
                    // Trả về null hoặc DTO rỗng tùy convention của bạn
                    return null;
                }

                // Lấy ID bước hiện tại từ Struct
                var currentStepId = projectStruct.CurrentStepWorkflowId;

                // BƯỚC 2: Tìm thông tin chi tiết của bước đó (Logic cũ)
                var _step = await _dbContext.PsProjectWorkflowProcessing
                                            .FirstOrDefaultAsync(x => x.Id == currentStepId);

                if (_step == null) return null;

                // BƯỚC 3: Map dữ liệu và xử lý Actions (Giữ nguyên logic cũ)
                var step = _mapper.Map<ProjectWorkflowProcessingDto>(_step);

                // Chuyển đổi chuỗi Action "1,2,3" thành List<int> [1, 2, 3]
                step.ListActions = step.Action?
                                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(int.Parse)
                                        .ToList();

                return step;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                // Log lỗi nếu cần
                return new ProjectWorkflowProcessingDto();
            }
        }
        public async Task TrinhDuyet(string code)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var projectstruct = await _dbContext.PsProjectStruct.FirstOrDefaultAsync(x => x.Code == code);
                if (projectstruct == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {code}";
                    return;
                }

                var currentStep = await _dbContext.PsProjectWorkflowProcessing
                    .FirstOrDefaultAsync(x => x.Id == projectstruct.CurrentStepWorkflowId);
                if (currentStep == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy bước xử lý hiện tại của quy trình workflow!";
                    return;
                }

                projectstruct.Status = ProjectStatus.DaTrinhDuyet;

                currentStep.IsDone = true;
                currentStep.IsProcessing = false;
                currentStep.Acted = WorkflowProjectAction.TrinhDuyet;

                if (!string.IsNullOrEmpty(currentStep.NextId))
                {
                    projectstruct.CurrentStepWorkflowId = currentStep.NextId;

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
                    projectstruct.CurrentStepWorkflowId = null;
                }

                _dbContext.PsProjectStruct.Update(projectstruct);
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

        public async Task XacNhan(string code)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProjectStruct.FirstOrDefaultAsync(x => x.Code == code);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {code}";
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

                project.Status = ProjectStatus.DaXacNhan;

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

                _dbContext.PsProjectStruct.Update(project);
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

        public async Task PheDuyet(string code)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProjectStruct.FirstOrDefaultAsync(x => x.Code == code);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {code}";
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

                project.Status = ProjectStatus.DaPheDuyet;

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

                _dbContext.PsProjectStruct.Update(project);
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

        public async Task TuChoi(string code)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProjectStruct.FirstOrDefaultAsync(x => x.Code == code);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {code}";
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

                project.Status = ProjectStatus.TuChoi;

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

                _dbContext.PsProjectStruct.Update(project);
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

        public async Task YeuCauChinhSua(string code)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.PsProjectStruct.FirstOrDefaultAsync(x => x.Code == code);
                if (project == null)
                {
                    this.Status = false;
                    this.MessageObject.Message = "Lỗi hệ thống!";
                    this.MessageObject.MessageDetail = $"Không tìm thấy dự án với ID: {code}";
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

                project.Status = ProjectStatus.YeuCauChinhSua;

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

                _dbContext.PsProjectStruct.Update(project);
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
