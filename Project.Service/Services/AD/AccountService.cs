using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Core;
using Project.Core.Entities.AD;
using Project.Service.Common;
using Project.Service.Dtos.AD;
using System.Security.Cryptography;

namespace Project.Service.Services.AD
{
    public interface IAccountService : IGenericService<AdAccount, AccountDto>
    {
        Task InsertAccount(AccountDto request);
        Task UpdateAccountRight(AccountRightDto request);
        Task<AccountDto> GetDetail(string userName);
    }

    public class AccountService(AppDbContext dbContext, IMapper mapper) : GenericService<AdAccount, AccountDto>(dbContext, mapper), IAccountService
    {
        public override async Task<PagedResponseDto> Search(AccountDto filter)
        {
            try
            {
                var query = _dbContext.AdAccount.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.UserName.ToString().Contains(filter.KeyWord) || x.FullName.Contains(filter.KeyWord));
                }

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

        private string CryptographyMD5(string source)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] bytHash = MD5.HashData(buffer);
            string result = string.Empty;
            foreach (byte a in bytHash)
            {
                result += int.Parse(a.ToString(), System.Globalization.NumberStyles.HexNumber).ToString();
            }
            return result;
        }

        public async Task InsertAccount(AccountDto request)
        {
            try
            {
                var entity = _mapper.Map<AdAccount>(request);
                entity.Password = CryptographyMD5(request.UserName);
                await _dbContext.AdAccount.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task<AccountDto> GetDetail(string userName)
        {
            try
            {
                var user = await _dbContext.AdAccount.FirstOrDefaultAsync(x => x.UserName == userName);
                if (user == null) return new AccountDto();

                var dto = _mapper.Map<AccountDto>(user);

                var lstAccountGroup = await _dbContext.AdAccountAccountGroup.Where(x => x.UserName == userName).Select(x => x.GroupId).ToListAsync();
                dto.AccountGroups = await _dbContext.AdAccountGroup.Where(x => lstAccountGroup.Contains(x.Id)).ToListAsync();

                var lstRights = await _dbContext.AdAccountGroupRight.Where(x => lstAccountGroup.Contains(x.GroupId)).Select(x => x.RightId).Distinct().ToListAsync();

                var lstAdded = await _dbContext.AdAccountRight.Where(x => x.UserName == userName && x.IsAdded == true).Select(x => x.RightId).ToListAsync();
                var lstRemoved = await _dbContext.AdAccountRight.Where(x => x.UserName == userName && x.IsRemoved == true).Select(x => x.RightId).ToListAsync();

                dto.Rights = lstRights.Union(lstAdded).Except(lstRemoved).Distinct().ToList();

                return dto;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return new AccountDto();
            }
        }

        public async Task UpdateAccountRight(AccountRightDto request)
        {
            try
            {
                var check = await _dbContext.AdAccountRight.FirstOrDefaultAsync(x => x.UserName == request.UserName && x.RightId == request.RightId);
                if (check == null)
                {
                    var ent = _mapper.Map<AdAccountRight>(request);
                    ent.Id = Guid.NewGuid().ToString();
                    await _dbContext.AdAccountRight.AddAsync(ent);
                }
                else
                {
                    check.IsAdded = request.IsAdded;
                    check.IsRemoved = request.IsRemoved;
                    _dbContext.AdAccountRight.Update(check);
                }
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
