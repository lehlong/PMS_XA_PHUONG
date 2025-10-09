using AutoMapper;
using Project.Core;
using Project.Core.Entities.AD;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.AD;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface ILoaiDuAnService : IGenericService<MdLoaiDuAn, LoaiDuAnDto>
    {
    }

    public class LoaiDuAnService(AppDbContext dbContext, IMapper mapper) : GenericService<MdLoaiDuAn, LoaiDuAnDto>(dbContext, mapper), ILoaiDuAnService
    {
        
    }
}
