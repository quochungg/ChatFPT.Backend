using ChatFPT.Core.Models.User;

namespace ChatFPT.Service.Interfaces
{
    public interface IAuthService
    {
        
        Task LoginGoogle(string token);

        Task<LoginResponse> Login(LoginRequestModel model);

        Task Register(RegisterRequestModel model);

        Task<ResponseUserModel> GetUserInfo();

        Task Delete(string id);

        
    }
}
