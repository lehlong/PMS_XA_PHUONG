using AutoMapper;
using Project.Core;
using Project.Core.Entities.AD;
using Project.Core.Entities.MD;
using Project.Service.Common;
using Project.Service.Dtos.AD;
using Project.Service.Dtos.MD;

namespace Project.Service.Services.MD
{
    public interface IConfigStructService : IGenericService<MdConfigStruct, ConfigStructDto>
    {
        Task UpdateOrder(List<ConfigStructDto> request);
    }

    public class ConfigStructService(AppDbContext dbContext, IMapper mapper) : GenericService<MdConfigStruct, ConfigStructDto>(dbContext, mapper), IConfigStructService
    {
        public override async Task<PagedResponseDto> Search(ConfigStructDto filter)
        {
            try
            {
                var query = _dbContext.MdConfigStruct.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.OrgId))
                {
                    query = query.Where(x => x.OrgId == filter.OrgId);
                }

                return await Paging(query, filter);

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
        public async Task UpdateOrder(List<ConfigStructDto> request)
        {
            try
            {
                var entity = _mapper.Map<List<MdConfigStruct>>(request);
                _dbContext.MdConfigStruct.UpdateRange(entity);
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
