namespace ZivnostAPI.Services.Generic;

public interface IReadOnlyService<T>
{
    Task<List<T>> GetAll();
    Task<T?> GetById(int id);
}
