using SharedTypesLibrary.ServiceResponseModel;

namespace ZivnostAPI.Services.Interfaces;

public interface IGenericReadOnlyService<T>
{
    Task<ApiResponse<T>> GetAll();
    Task<ApiResponse<T?>> GetById(int id);
}
