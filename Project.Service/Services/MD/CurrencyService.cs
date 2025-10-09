using AutoMapper;
using Project.Core;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface ICurrencyService : IGenericService<MdCurrency, CurrencyDto>
    {
    }

    public class CurrencyService(AppDbContext dbContext, IMapper mapper) : GenericService<MdCurrency, CurrencyDto>(dbContext, mapper), ICurrencyService
    {

    }
}
