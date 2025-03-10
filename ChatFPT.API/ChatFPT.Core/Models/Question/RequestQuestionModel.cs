namespace ChatFPT.Core.Models.Question
{
    public class RequestQuestionModel
    {
        public string? Content { get; set; }

        public string? CategoryId { get; set; }

        public bool IsResolve { get; set; }
    }
}
