using Microsoft.AspNetCore.Identity;

namespace ChatFPT.Domain.Entities
{
    public class ApplicationUserRoles : IdentityUserRole<Guid>
    {
        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset LastUpdatedTime { get; set; }

        public DateTimeOffset? DeletedTime { get; set; }

    }
}
