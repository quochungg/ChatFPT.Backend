namespace ChatFPT.Core.Models.Question
{
    public class RequestQuestionModel
    {
        public string? Content { get; set; }

        public string? TagId { get; set; }

        public bool IsResolve { get; set; }
    }
}
