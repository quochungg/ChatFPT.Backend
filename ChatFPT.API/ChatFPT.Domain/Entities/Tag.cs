using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Tag : AuditableEntity
    {
        public string? Name { get; set; }

    }
}
