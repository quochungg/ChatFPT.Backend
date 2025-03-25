using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatFPT.Core.Models.Feedback
{
    public class ResponseFeedbackModel
    {
        public string? AnswerId { get; set; }

        public string? QuestionId { get; set; }
        
        public string? AnswerContent { get; set; }

        public string? QuestionContent { get; set; }

        public string? Note {  get; set; }

        public int? Rate { get; set; }

        public DateTimeOffset? CreatedTime { get; set; }

    }
}
