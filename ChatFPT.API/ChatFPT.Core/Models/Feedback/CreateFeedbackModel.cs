using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ChatFPT.Core.Models.Feedback
{
    public class CreateFeedbackModel
    {
        public string answerId { get; set; }
        public int? Rate { get; set; }
    }

}
