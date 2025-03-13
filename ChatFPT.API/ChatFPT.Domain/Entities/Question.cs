using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Question : AuditableEntity
    {
        
        public string? Content { get; set; }

        public bool IsResolve { get; set; }

    }
}
