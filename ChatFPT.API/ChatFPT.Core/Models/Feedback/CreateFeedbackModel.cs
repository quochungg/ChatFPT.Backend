

namespace ChatFPT.Core.Models.Feedback
{
    public class CreateFeedbackModel
    {
        public string? AnswerId { get; set; }
        public int? Rate { get; set; }

        public string? Note { get; set; }
    }

}
