using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class Answer : AuditableEntity
    {
        public string? QuestionId { get; set; }
        public virtual Question? Question { get; set; }
        public string? Content { get; set; }

    }
}
