

namespace ChatFPT.Service.Interfaces
{
    public interface IRedisService
    {
        public T? GetData<T>(string key);

        void SetData<T>(string key, T data);
    }
}
