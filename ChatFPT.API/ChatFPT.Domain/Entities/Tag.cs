using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Tag : AuditableEntity
    {
        public string? Name { get; set; }

        public string? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

    }
}
