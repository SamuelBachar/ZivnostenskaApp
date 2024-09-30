using Microsoft.EntityFrameworkCore;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Services.Generic;

public class GenericWriteService<T> : IGenericWriteService<T> where T : class
{
    private readonly CusDbContext _dbContext;
    public GenericWriteService(CusDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Add(T entity)
    {
        bool result = false;
        await _dbContext.Set<T>().AddAsync(entity);
        result = await _dbContext.SaveChangesAsync() > 0;

        return result;
    }

    public async Task<bool> Update(int id, T entity)
    {
        bool result = false;

        T? dbEntity = await _dbContext.Set<T>().FindAsync(id);

        if (dbEntity != null)
        {
            _dbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
            result = await _dbContext.SaveChangesAsync() > 0;
        }

        return result;
    }

    public async Task<bool> Delete(int id)
    {
        bool result = false;

        var entity = await _dbContext.Set<T>().FindAsync(id);
        
        if (entity != null)
        {
            _dbContext.Set<T>().Remove(entity);
            result = await _dbContext.SaveChangesAsync() > 0;
        }

        return result;
    }
}
