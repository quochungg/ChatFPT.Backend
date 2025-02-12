using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Category : AuditableEntity
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
    }
}
