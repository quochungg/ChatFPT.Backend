using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Category : AuditableEntity
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
    }
}
