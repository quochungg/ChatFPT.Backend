

namespace ChatFPT.Core.Models.Role
{
    public class ResponseRoleClaimModel
    {
        public string? Id { get; set; }

        public string? RoleName { get; set; }
        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public DateTimeOffset? CreatedTime { get; set; }
    }
}
