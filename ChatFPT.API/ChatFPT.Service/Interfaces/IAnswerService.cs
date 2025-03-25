using ChatFPT.Core.Models.Answer;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface IAnswerService
    {
        Task<PaginatedList<ResponseAnswerModel>> GetAllAnswers(string? searchName, int index = 1, int pageSize = 10);

        Task CreateAnswer(CreateAnswerModel model);

        Task UpdateAnswer(UpdateAnswerModel model);

        Task DeleteAnswer(string? answerId);

        Task<ResponseAnswerModel> GetAnswerById(string id);

        Task<ResponseAnswerModel> GetAnswerByQuestionId(string QuestionId);
    }
}
