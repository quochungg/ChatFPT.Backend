

using ChatFPT.Core.Models.Question;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface IQuestionService
    {
        Task<PaginatedList<ResponseQuestionModel>> GetAllQuestion(string? searchName, int index = 1, int PageSize = 10);

        Task<ResponseQuestionModel> GetQuestionById(string id);

        Task CreateQuestion(RequestQuestionModel model);

        Task UpdateQuestion(UpdateQuestionModel model);

        Task DeleteQuestion(string id);
    }
}
