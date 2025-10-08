using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Project.Core;
using Project.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Common
{
    public interface IGenericService<TEntity, TDto> : IBaseService
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        DbContext GetDbContext();
        Task<PagedResponseDto> Search(TDto filter);
        Task<IList<TDto>> GetAll();
        Task<TDto> GetById(object id);
        Task<TDto> Add(IDto dto);
        Task Update(IDto dto);
        Task Delete(object code);
        Task<PagedResponseDto> Paging(IQueryable<TEntity> query, TDto filter);


    }
    public abstract class GenericService<TEntity, TDto>(AppDbContext dbContext, IMapper mapper) : BaseService(dbContext, mapper), IGenericService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        public DbContext GetDbContext()
        {
            return _dbContext;
        }

        public virtual async Task<PagedResponseDto> Search(TDto filter)
        {
            try
            {
                var query = _dbContext.Set<TEntity>().AsQueryable();

                return await Paging(query, filter);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public virtual async Task<IList<TDto>> GetAll()
        {
            try
            {
                var query = _dbContext.Set<TEntity>();
                var lstEntity = await _dbContext.Set<TEntity>().ToListAsync();
                return _mapper.Map<List<TDto>>(lstEntity);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public virtual async Task<TDto> GetById(object id)
        {
            try
            {
                var entity = await _dbContext.Set<TEntity>().FindAsync(id);
                return _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public virtual async Task<TDto> Add(IDto dto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(dto);

                var keyProperties = dto.GetType()
                    .GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
                    .ToList();

                if (keyProperties.Any())
                {
                    var query = _dbContext.Set<TEntity>().AsQueryable();

                    foreach (var prop in keyProperties)
                    {
                        var keyName = prop.Name;
                        var keyValue = prop.GetValue(dto);

                        if (keyValue == null) continue;

                        var parameter = Expression.Parameter(typeof(TEntity), "e");
                        var left = Expression.Call(
                            typeof(EF),
                            nameof(EF.Property),
                            new[] { typeof(object) },
                            parameter,
                            Expression.Constant(keyName)
                        );
                        var right = Expression.Constant(keyValue);
                        var equal = Expression.Equal(left, right);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

                        query = query.Where(lambda);
                    }

                    if (await query.AnyAsync())
                    {
                        throw new InvalidOperationException("Mã đã tồn tại! Vui lòng nhập mã khác!");
                    }
                }

                var entityResult = await _dbContext.Set<TEntity>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var dtoResult = _mapper.Map<TDto>(entityResult.Entity);
                return dtoResult;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
        public virtual async Task Delete(object code)
        {
            try
            {
                var entity = _dbContext.Set<TEntity>().Find(code);
                if (entity == null)
                {
                    Status = false;
                    MessageObject.Code = "0000";
                    return;
                }
                _dbContext.Entry<TEntity>(entity).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        private static PropertyInfo? GetKeyField(IDto dto)
        {
            PropertyInfo keyProperty = null;
            Type t = dto.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object[] attrs = pi.GetCustomAttributes(typeof(KeyAttribute), false);
                if (attrs != null && attrs.Length == 1)
                {
                    keyProperty = pi;
                    break;
                }
            }
            return keyProperty;
        }
        private static object? GetValueOfKeyField(IDto dto, PropertyInfo keyProperty)
        {
            object value = null;
            if (keyProperty != null)
            {
                value = keyProperty.GetValue(dto, null);
            }
            return value;
        }

        public virtual async Task Update(IDto dto)
        {
            try
            {
                var keyField = GenericService<TEntity, TDto>.GetKeyField(dto);
                if (keyField == null)
                {
                    Status = false;
                    MessageObject.Code = "0002";
                    return;
                }
                var keyValue = GenericService<TEntity, TDto>.GetValueOfKeyField(dto, keyField);
                var entityInDB = await _dbContext.Set<TEntity>().FindAsync(keyValue);
                if (entityInDB == null)
                {
                    Status = false;
                    MessageObject.Code = "0003";
                    return;
                }
                _mapper.Map(dto, entityInDB);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public virtual async Task<PagedResponseDto> Paging(IQueryable<TEntity> query, TDto filter)
        {
            try
            {
                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize);

                var result = filter.IsPaging == true ?
                    await query.Skip((filter.CurrentPage - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync() :
                    await query.ToListAsync();

                return new PagedResponseDto
                {
                    Data = _mapper.Map<List<TDto>>(result),
                    TotalRecord = totalRecords,
                    TotalPage = totalPages,
                    CurrentPage = filter.CurrentPage,
                    PageSize = filter.PageSize
                };
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
    }
}
