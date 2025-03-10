using ChatFPT.Domain.Entities;

namespace ChatFPT.Core.Models.User
{
    public class LoginQueryModel
    {
        public ApplicationUser? User { get; set; }

        public string? RoleName { get; set; }
    }
}
