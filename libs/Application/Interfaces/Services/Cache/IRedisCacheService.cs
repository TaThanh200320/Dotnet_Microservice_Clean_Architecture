namespace Application.Interfaces.Services.Cache;

public interface IDistributedCacheService : ICacheService
{
    Task RemoveAsync(string key);
}
