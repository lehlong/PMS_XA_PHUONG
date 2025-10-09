using AutoMapper;
using Project.Core;
using Project.Core.Entities.AD;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.AD;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface IUnitService : IGenericService<MdUnit, UnitDto>
    {
    }

    public class UnitService(AppDbContext dbContext, IMapper mapper) : GenericService<MdUnit, UnitDto>(dbContext, mapper), IUnitService
    {

    }
}
