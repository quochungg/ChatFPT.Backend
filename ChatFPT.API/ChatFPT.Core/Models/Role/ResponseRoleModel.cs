

namespace ChatFPT.Core.Models.Role
{
    public class ResponseRoleModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? FullName { get; set; }

        public DateTimeOffset? CreatedTime { get; set; }
    }
}
