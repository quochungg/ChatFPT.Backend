namespace ChatFPT.Core.Models.AI
{
    public class UploadDataModel
    {
        public string Document { get; set; } = string.Empty;
        public List<string> TagId { get; set; } = new List<string>();
    }
}
