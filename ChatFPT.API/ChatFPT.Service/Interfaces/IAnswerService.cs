using ChatFPT.Core.Models.Answer;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface IAnswerService
    {
        Task<PaginatedList<ResponseAnswerModel>> GetAllAnswers(string? searchName, int index, int pageSize, string orderBy, string sortBy);

        Task CreateAnswer(CreateAnswerModel model);

        Task UpdateAnswer(UpdateAnswerModel model);

        Task DeleteAnswer(string? answerId);

        Task<ResponseAnswerModel> GetAnswerById(string id);

        Task<ResponseAnswerModel> GetAnswerByQuestionId(string QuestionId);
    }
}
