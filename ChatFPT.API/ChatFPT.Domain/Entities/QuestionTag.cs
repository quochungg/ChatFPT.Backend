

namespace ChatFPT.Domain.Entities
{
    public class QuestionTag
    {
        public string? QuestionId { get; set; }
        public virtual Question? Question { get; set; }
        public string? TagId { get; set; }

        public virtual Tag? Tag { get; set; }
    }
}
