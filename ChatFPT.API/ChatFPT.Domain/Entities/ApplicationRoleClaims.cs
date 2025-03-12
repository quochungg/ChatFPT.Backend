

using Microsoft.AspNetCore.Identity;

namespace ChatFPT.Domain.Entities
{
    public class ApplicationRoleClaims : IdentityRoleClaim<Guid>
    { 
        public DateTimeOffset? CreatedTime { get; set; }

        public DateTimeOffset? LastUpdatedTime { get; set; }

        public DateTimeOffset? DeletedTime { get; set; }

        public ApplicationRoleClaims()
        {
            CreatedTime = DateTimeOffset.UtcNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
