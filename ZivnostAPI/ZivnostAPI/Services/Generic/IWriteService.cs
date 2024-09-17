namespace ZivnostAPI.Services.Generic;

public interface IWriteService<T>
{
    Task<bool> Add(T entity);
    Task<bool> Update(int id, T entity);
    Task<bool> Delete(int id);
}
