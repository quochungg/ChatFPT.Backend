

namespace ChatFPT.Service.Interfaces
{
    public interface IFcmService
    {
        public  Task SendNotificationAsync(string deviceToken, string title, string body);
    }
}
