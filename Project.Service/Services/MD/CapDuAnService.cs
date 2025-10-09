using AutoMapper;
using Project.Core;
using Project.Core.Entities.AD;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.AD;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface ICapDuAnService : IGenericService<MdCapDuAn, CapDuAnDto>
    {
    }

    public class CapDuAnService(AppDbContext dbContext, IMapper mapper) : GenericService<MdCapDuAn, CapDuAnDto>(dbContext, mapper), ICapDuAnService
    {

    }
}
