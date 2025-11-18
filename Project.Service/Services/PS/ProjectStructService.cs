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
                            ProjectId = request.ProjectId,
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
                if (request == null || request.Count == 0)
                {
                    return; 
                }
                var firstItem = request.FirstOrDefault();
                string taskId = firstItem.TaskId;
                string projectId = firstItem.ProjectId;

                if (string.IsNullOrEmpty(taskId))
                {
                    this.Status = false;
                    return;
                }
                var existingList = await _dbContext.PsTaskPerson
                                                   .Include(x => x.TaskPersonDetails)
                                                   .Where(x => x.TaskId == taskId)
                                                   .ToListAsync();
                var requestIds = request.Where(x => !string.IsNullOrEmpty(x.Id)).Select(x => x.Id).ToList();
                var itemsToDelete = existingList.Where(x => !requestIds.Contains(x.Id)).ToList();

                if (itemsToDelete.Any())
                {
                    _dbContext.PsTaskPerson.RemoveRange(itemsToDelete);
                }
                foreach (var itemDto in request)
                {
                    itemDto.TaskId = taskId;
                    itemDto.ProjectId = projectId;

                    var existingItem = existingList.FirstOrDefault(x => x.Id == itemDto.Id);

                    if (existingItem == null)
                    {
                        var newItem = _mapper.Map<PsTaskPerson>(itemDto);

                        newItem.Id = Guid.NewGuid().ToString(); 
                        newItem.TaskId = taskId;
                        newItem.ProjectId = projectId;
                        if (itemDto.TaskRoles != null && itemDto.TaskRoles.Count > 0)
                        {
                            newItem.TaskRoles = string.Join(",", itemDto.TaskRoles);
                        }

                        if (newItem.TaskPersonDetails != null)
                        {
                            foreach (var detail in newItem.TaskPersonDetails)
                            {
                                detail.Id = Guid.NewGuid().ToString();
                            }
                        }

                        await _dbContext.PsTaskPerson.AddAsync(newItem);
                    }
                    else
                    {
                        existingItem.UserName = itemDto.UserName;
                        existingItem.ProjectId = projectId;
                        if (itemDto.TaskRoles != null && itemDto.TaskRoles.Count > 0)
                        {
                            existingItem.TaskRoles = string.Join(",", itemDto.TaskRoles);
                        }
                        else
                        {
                            existingItem.TaskRoles = null;
                        }
                        if (itemDto.TaskPersonDetails != null)
                        {
                            var existingDetails = existingItem.TaskPersonDetails.ToList();
                            var reqDetailIds = itemDto.TaskPersonDetails
                                                .Where(d => !string.IsNullOrEmpty(d.Id))
                                                .Select(d => d.Id).ToList();
                            var detailsToDelete = existingDetails.Where(d => !reqDetailIds.Contains(d.Id)).ToList();
                            foreach (var d in detailsToDelete)
                            {
                                _dbContext.PsTaskPersonDetail.Remove(d);
                            }
                            foreach (var detailDto in itemDto.TaskPersonDetails)
                            {
                                var existingDetail = existingDetails.FirstOrDefault(d => d.Id == detailDto.Id);

                                if (existingDetail == null)
                                {
                                    // Thêm Detail mới
                                    var newDetail = _mapper.Map<PsTaskPersonDetail>(detailDto);
                                    newDetail.Id = Guid.NewGuid().ToString();
                                    newDetail.TaskPersonId = existingItem.Id; // Gán vào cha hiện tại
                                    existingItem.TaskPersonDetails.Add(newDetail);
                                }
                                else
                                {
                                    // Sửa Detail cũ
                                    existingDetail.Task = detailDto.Task;
                                    existingDetail.Note = detailDto.Note;
                                }
                            }
                        }
                    }
                }

                // 5. Lưu DB
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
