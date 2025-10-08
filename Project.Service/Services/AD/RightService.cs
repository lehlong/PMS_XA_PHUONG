using AutoMapper;
using Project.Core;
using Project.Core.Entities.AD;
using Project.Service.Common;
using Project.Service.Dtos.AD;

namespace Project.Service.Services.AD
{
    public interface IRightService : IGenericService<AdRight, RightDto>
    {
        Task UpdateOrder(List<RightDto> request);
    }

    public class RightService(AppDbContext dbContext, IMapper mapper) : GenericService<AdRight, RightDto>(dbContext, mapper), IRightService
    {
        public async Task UpdateOrder(List<RightDto> request)
        {
            try
            {
                var entity = _mapper.Map<List<AdRight>>(request);
                _dbContext.AdRight.UpdateRange(entity);
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
