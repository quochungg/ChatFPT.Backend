using ChatFPT.Core.Models.AI;

namespace ChatFPT.Service.Interfaces
{
    public interface IAIService
    {
        Task<List<float>> GetEmbeddingAsync(string text);
        Task<bool> UploadDataToPineconeAsync(List<UploadDataModel> model);
        Task<string> QueryDataAsync(string question);
    }
}
