namespace ZivnostAPI.Services.Generic;

public interface ICrudService<T> : IReadOnlyService<T>, IWriteService<T>
{
}
