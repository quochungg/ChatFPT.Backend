

using ChatFPT.Service.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatFPT.Service.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;

        }

        public T? GetData<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void SetData<T>(string key, T data)
        {
            throw new NotImplementedException();
        }
    }
}
