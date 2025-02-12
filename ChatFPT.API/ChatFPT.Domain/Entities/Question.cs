using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Question : AuditableEntity
    {
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public string? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        
        public string? Content { get; set; }

        public bool IsResolve { get; set; }

    }
}
