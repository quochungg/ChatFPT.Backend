using ChatFPT.Core.Models.User;

namespace ChatFPT.Service.Interfaces
{
    public interface IAuthService
    {
        Task LoginGoogle(string token);

        Task Login(string username, string password);

        Task<UserInfoModel> GetUserInfo();

        void Delete(string id);
    }
}
