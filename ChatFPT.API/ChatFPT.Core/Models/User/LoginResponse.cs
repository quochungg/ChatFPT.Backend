

namespace ChatFPT.Core.Models.User
{
    public class LoginResponse
    {
        public TokenResponse? TokenResponse { get; set; }

        public string? DeviceToken { get; set; }
    }
}
