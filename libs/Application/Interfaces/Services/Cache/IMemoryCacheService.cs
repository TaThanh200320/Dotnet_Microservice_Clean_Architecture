namespace Application.Interfaces.Services.Cache;

public interface IMemoryCacheService : ICacheService
{
    bool HasKey(string key);
}
