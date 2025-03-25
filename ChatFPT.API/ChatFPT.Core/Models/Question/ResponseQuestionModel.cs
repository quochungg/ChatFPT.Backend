namespace ChatFPT.Core.Models.Question
{
    public class ResponseQuestionModel
    {
        public string? Id { get; set; }
        public string? TagId { get; set; }
        public string? TagName { get; set; }
        public string? Content { get; set; }

        public bool IsResolve { get; set; }
    }
}
