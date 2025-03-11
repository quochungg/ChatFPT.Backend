

namespace ChatFPT.Core.Models.Tag
{
    public class ResponseTagModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? CategoryId { get; set; }

        public DateTimeOffset? CreatedTime { get; set; }
    }
}
