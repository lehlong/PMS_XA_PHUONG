using AutoMapper;
using Project.Core;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface ICustomerService : IGenericService<MdCustomer, CustomerDto>
    {
    }

    public class CustomerService(AppDbContext dbContext, IMapper mapper) : GenericService<MdCustomer, CustomerDto>(dbContext, mapper), ICustomerService
    {

    }
}
