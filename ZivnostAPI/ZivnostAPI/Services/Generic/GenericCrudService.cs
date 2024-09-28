using Microsoft.EntityFrameworkCore;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.CusDbContext;

namespace ZivnostAPI.Services.Generic;

public class GenericCrudService<T> : ICrudService<T> where T : class
{
    private readonly IReadOnlyService<T> _readOnlyService;
    private readonly IWriteService<T> _writeService;

    public GenericCrudService(IReadOnlyService<T> readOnlyService, IWriteService<T> writeService)
    {
        _writeService = writeService;
        _readOnlyService = readOnlyService;
    }
    
    public async Task<ApiResponse<List<T>>> GetAll()
    {
        return await _readOnlyService.GetAll();
    }

    public async Task<ApiResponse<T?>> GetById(int id)
    {
        return await _readOnlyService.GetById(id);
    }

    public async Task<bool> Add(T entity)
    {
        return await _writeService.Add(entity);
    }

    public async Task<bool> Update(int id, T entity)
    {
        return await _writeService.Update(id, entity);
    }

    public async Task<bool> Delete(int id)
    {
        return await _writeService.Delete(id);
    }
}
