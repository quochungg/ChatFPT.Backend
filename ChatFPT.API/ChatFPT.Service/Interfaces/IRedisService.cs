

namespace ChatFPT.Service.Interfaces
{
    public interface IRedisService
    {
        Task SetCacheResponseAsync(string key, object reponse, TimeSpan timeOut);
        Task<string?> GetCacheResponseAsync(string key);
        Task RemoveCacheResponseAsync(string pattern);
    }
}
