using Microsoft.EntityFrameworkCore;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Services.Generic;

public class GenericCrudService<T> : IGenericCrudService<T> where T : class
{
    private readonly IGenericReadOnlyService<T> _readOnlyService;
    private readonly IGenericWriteService<T> _writeService;

    public GenericCrudService(IGenericReadOnlyService<T> readOnlyService, IGenericWriteService<T> writeService)
    {
        _writeService = writeService;
        _readOnlyService = readOnlyService;
    }
    
    public async Task<ApiResponse<T>> GetAll()
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
