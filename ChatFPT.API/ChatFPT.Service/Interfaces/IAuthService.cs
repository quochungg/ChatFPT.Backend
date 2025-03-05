using ChatFPT.Core.Models.User;

namespace ChatFPT.Service.Interfaces
{
    public interface IAuthService
    {
        
        Task LoginGoogle(string token);

        Task Login(LoginRequestModel model);

        Task Register(RegisterRequestModel model);

        Task<UserInfoModel> GetUserInfo();

        Task Delete(string id);

        
    }
}
