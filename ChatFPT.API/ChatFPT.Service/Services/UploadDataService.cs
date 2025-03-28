using ChatFPT.Core.Constaints;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pinecone;
//using Pinecone.Grpc;
using System.Text;
using System.Text.Json;
using static ChatFPT.Core.Base.BaseException;

public class UploadDataService : IUploadDataService
{
    private readonly string _pineconeApiKey;
    private readonly string _openaiApiKey;
    private readonly string _indexName;
    private readonly HttpClient _httpClient;

    public UploadDataService(IConfiguration configuration)
    {
        _pineconeApiKey = configuration["PineCone:PineconeApiKey"]
            ?? throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không tìm thấy Pinecone API Key");
        _openaiApiKey = configuration["OpenAI:ApiKey"]
            ?? throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không tìm thấy OpenAI API Key");
        _indexName = configuration["PineCone:PineconeIndexName"]
            ?? throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không tìm thấy Pinecone Index");

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Api-Key", _pineconeApiKey);
    }

    public async Task<List<float>> GetEmbeddingAsync(string text)
    {
        var requestBody = new
        {
            model = "text-embedding-3-small",
            input = text
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openaiApiKey}");

        HttpResponseMessage response = await _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, $"Lỗi lấy embeddings từ OpenAI: {response.ReasonPhrase}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(responseJson);
        var embedding = doc.RootElement.GetProperty("data")[0].GetProperty("embedding");

        List<float> vector = new List<float>();
        foreach (var value in embedding.EnumerateArray())
        {
            vector.Add(value.GetSingle());
        }

        return vector;
    }

    public async Task<bool> UploadDataToPineconeAsync(List<string> documents)
    {
        if (documents == null || documents.Count == 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Dữ liệu không được để trống");
        }

        var client = new PineconeClient(_pineconeApiKey);
        var index = client.Index(_indexName);

        if (index == null)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Index name không tồn tại");
        }

        List<Vector> vectors = new List<Vector>();
        foreach (var document in documents)
        {
            var embedding = await GetEmbeddingAsync(document);
            if (embedding == null)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không thể tạo embedding cho dữ liệu.");
            }

            vectors.Add(new Vector
            {
                Id = Guid.NewGuid().ToString(),
                Values = embedding.ToArray(),
                Metadata = new Metadata { { "text", document } }
            });
        }

        var upsertRequest = new UpsertRequest
        {
            Vectors = vectors
        };

        await index.UpsertAsync(upsertRequest);

        return true;
    }

}
