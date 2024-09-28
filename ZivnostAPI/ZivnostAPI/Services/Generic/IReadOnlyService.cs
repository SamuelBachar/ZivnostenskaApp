using SharedTypesLibrary.ServiceResponseModel;

namespace ZivnostAPI.Services.Generic;

public interface IReadOnlyService<T>
{
    Task<ApiResponse<List<T>>> GetAll();
    Task<ApiResponse<T?>> GetById(int id);
}
