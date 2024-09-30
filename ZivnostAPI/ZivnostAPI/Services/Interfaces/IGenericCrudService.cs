namespace ZivnostAPI.Services.Interfaces;

public interface IGenericCrudService<T> : IGenericReadOnlyService<T>, IGenericWriteService<T>
{
}
