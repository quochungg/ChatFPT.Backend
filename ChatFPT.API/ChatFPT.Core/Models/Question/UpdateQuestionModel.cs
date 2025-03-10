namespace ChatFPT.Core.Models.Question
{
    public class UpdateQuestionModel
    {
        public string? Id { get; set; }
        public string? Content { get; set; }

        public string? CategoryId { get; set; }

        public bool IsResolve { get; set; }
    }
}
