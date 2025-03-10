

namespace ChatFPT.Core.Models.User
{
    public class ResponseUserModel
    {
        public string? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? MSSV { get; set; }

        public string? Role { get; set; }

        public DateTimeOffset? CreatedTime { get; set; }
    }
}
