namespace ChatFPT.Core.Models.User
{
    public class LoginRequestModel
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }

        public string? DeviceToken { get; set; }
    }
}
