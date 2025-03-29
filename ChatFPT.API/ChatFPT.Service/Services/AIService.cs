using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.AI;
using ChatFPT.Core.Models.Answer;
using ChatFPT.Core.Models.Question;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pinecone;
using System.Text;
using System.Text.Json;

public class AIService : IAIService
{
    private readonly string _pineconeApiKey;
    private readonly string _openaiApiKey;
    private readonly string _indexName;
    private readonly string _model;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public AIService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
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
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

    public async Task<ResponseAnswerModel> QueryDataAsync(string question)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openaiApiKey}");

        string query = await QueryPinecone(question);

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "Bạn là trợ lý ảo chuyên hỗ trợ sinh viên Đại học FPT. Hãy cung cấp câu trả lời chính xác và hữu ích dựa trên thông tin sau:\n" + query +
                    "Bạn có thể hỗ trợ về:\n" +
                    "- Thông tin về chương trình học, lịch học, đăng ký môn học.\n" +
                    "- Quy trình học tập, quy định học vụ, học phí.\n" +
                    "- Hỗ trợ về tài khoản sinh viên, email, hệ thống LMS.\n" +
                    "- Câu lạc bộ, sự kiện, cơ hội học bổng, trao đổi sinh viên.\n\n" +
                    "Nếu sinh viên có nhu cầu thực hiện các thủ tục, dịch vụ vui lòng liên hệ Trung tâm Dịch vụ Sinh viên tại Phòng 202, điện thoại : 028.73005585 , email: sschcm@fe.edu.vn\n"
                    },
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
        
        var answer = jsonResponse!.choices[0].message.content.ToString();

        var questionDb = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(q => q.Content! == question) ?? throw new Exception("Không tìm thấy câu hỏi");

        await _unitOfWork.GetRepository<Answer>().AddAsync(new Answer
        {
            Content = answer,
            CreatedTime = DateTime.Now,
            QuestionId = questionDb.Id,
            Question = questionDb

        });

        await _unitOfWork.SaveAsync();

        var result = _mapper.Map<ResponseAnswerModel>(_unitOfWork.GetRepository<Answer>().Entities.FirstOrDefault(a => a.QuestionId == questionDb.Id));

        return result;
    }

    public async Task<bool> UploadDataToPineconeAsync(List<UploadDataModel> model)
    {
        if (model.Count == 0)
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

        foreach (var document in model)
        {
            var embedding = await GetEmbeddingAsync(document.Document);
            if (embedding == null)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không thể tạo embedding cho dữ liệu.");
            }

            string tag = string.Join(",", document.TagId);

            vectors.Add(new Vector
            {
                Id = Guid.NewGuid().ToString(),
                Values = embedding.ToArray(),
                Metadata = new Metadata { { "text", document.Document }, { "tags", tag } }
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
            return null;
        }

        var embedding = await GetEmbeddingAsync(query);
        if (embedding == null) return null;

        var queryResponse = await index.QueryAsync(new QueryRequest
        {
            Vector = embedding.ToArray(),
            TopK = 1,
            IncludeMetadata = true
        }
        ) ?? throw new Exception("");

        if (queryResponse.Matches!.Count() == 0)
            return null;

        //Xử lý tag
        var tagsMetaData = queryResponse.Matches!.Select(m => m.Metadata!["tags"]).FirstOrDefault() ?? throw new Exception("Error Tag");

        var tags = tagsMetaData.Value.ToString();

        var question = new Question
        {
            Content = query,
            CreatedTime = DateTime.Now,
            IsResolve = true
        };

        await _unitOfWork.GetRepository<Question>().AddAsync(question);

        await _unitOfWork.SaveAsync();

        question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(q => q.Content == query) ?? throw new Exception("");

        foreach (var tag in tags!.Split(","))
        {
            await _unitOfWork.GetRepository<QuestionTag>().AddAsync(new QuestionTag
            {
                QuestionId = question.Id,
                TagId = tag,
                CreatedTime = DateTime.Now
            });

            await _unitOfWork.SaveAsync();
        }

        return string.Join("\n", queryResponse.Matches!.Select(m => m.Metadata["text"]));
    }


}

