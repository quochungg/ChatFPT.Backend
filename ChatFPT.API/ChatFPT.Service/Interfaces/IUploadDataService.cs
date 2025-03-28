namespace ChatFPT.Service.Interfaces
{
    public interface IUploadDataService
    {
        Task<List<float>> GetEmbeddingAsync(string text);
        Task<bool> UploadDataToPineconeAsync(List<string> documents);
        //Task<string> QueryDataAsync(string query);
    }
}
