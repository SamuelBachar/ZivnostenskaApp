namespace ZivnostAPI.Services.Interfaces;

public interface IGenericWriteService<T>
{
    Task<bool> Add(T entity);
    Task<bool> Update(int id, T entity);
    Task<bool> Delete(int id);
}
