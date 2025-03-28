using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pinecone;
//using Pinecone.Grpc;
using System.Text;
using System.Text.Json;

public class UploadDataService : IUploadDataService
{
    private readonly string _pineconeApiKey;
    private readonly string _openaiApiKey;
    private readonly string _indexName;
    private readonly string _model;
    private readonly HttpClient _httpClient;

    public UploadDataService(IConfiguration configuration)
    {
        _pineconeApiKey = configuration["PineCone:PineconeApiKey"]
            ?? throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không tìm thấy Pinecone API Key");
        _openaiApiKey = configuration["OpenAI:ApiKey"]
            ?? throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không tìm thấy OpenAI API Key");
        _indexName = configuration["PineCone:PineconeIndexName"]
            ?? throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không tìm thấy Pinecone Index");
        _model = configuration["OpenAI:OpenAIModel"]
            ?? throw new Exception("Model not found in environment variables!");
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

    public async Task<string> QueryDataAsync(string question)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openaiApiKey}");

        string query = await QueryPinecone(question);

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "Bạn là trợ lý ảo chuyên hỗ trợ sinh viên Đại học FPT. Hãy cung cấp câu trả lời chính xác, ngắn gọn và hữu ích dựa trên thông tin sau:\n" + query +
                    "Bạn có thể hỗ trợ về:\n" +
                    "- Thông tin về chương trình học, lịch học, đăng ký môn học.\n" +
                    "- Quy trình học tập, quy định học vụ, học phí.\n" +
                    "- Hỗ trợ về tài khoản sinh viên, email, hệ thống LMS.\n" +
                    "- Câu lạc bộ, sự kiện, cơ hội học bổng, trao đổi sinh viên.\n\n" +
                    "Nếu không chắc chắn về câu trả lời, hãy hướng dẫn sinh viên liên hệ bộ phận phù hợp." },
                new { role = "user", content = question }
            }
        };

        var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {         
            return null;
        }

        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        return jsonResponse.choices[0].message.content.ToString();
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
    public async Task<string> QueryPinecone(string query)
    {
        var client = new PineconeClient(_pineconeApiKey);
        var index = client.Index(_indexName);

        if (index == null)
        {
            Console.WriteLine("❌ Không tìm thấy index trong Pinecone.");
            return null;
        }

        // Mã hóa câu hỏi thành vector bằng OpenAI (hoặc dùng SentenceTransformer)
        var embedding = await GetEmbeddingAsync(query);
        if (embedding == null) return null;



        // Truy vấn Pinecone
        var queryResponse = await index.QueryAsync(new QueryRequest
        {
            Vector = embedding.ToArray(),
            TopK = 3,
            IncludeMetadata = true
        }
        );

        if (queryResponse.Matches.Count() == 0)
            return null;

        return string.Join("\n", queryResponse.Matches.Select(m => m.Metadata["text"]));
    }


}

