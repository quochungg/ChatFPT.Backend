using ChatFPT.Core.Models.AI;
using ChatFPT.Core.Models.Answer;

namespace ChatFPT.Service.Interfaces
{
    public interface IAIService
    {
        Task<List<float>> GetEmbeddingAsync(string text);
        Task<bool> UploadDataToPineconeAsync(List<UploadDataModel> model);
        Task<ResponseAnswerModel> QueryDataAsync(string question);
    }
}
