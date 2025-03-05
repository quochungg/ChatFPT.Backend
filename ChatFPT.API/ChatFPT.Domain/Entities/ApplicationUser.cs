
using Microsoft.AspNetCore.Identity;

namespace ChatFPT.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? MSSV { get; set; }
        public string? Password { get; set; }

        public bool isGoogle { get; set; }

        public string? GoogleId { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }

        public DateTimeOffset? LastUpdatedTime { get; set; }

        public DateTimeOffset? DeletedTime { get; set; }

        public ApplicationUser()
        {
            CreatedTime = DateTimeOffset.UtcNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
