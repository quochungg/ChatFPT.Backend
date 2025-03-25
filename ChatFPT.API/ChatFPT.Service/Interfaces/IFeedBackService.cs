using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Feedback;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface IFeedBackService
    {
        Task<PaginatedList<ResponseFeedbackModel>> GetFeedbacksAsync(string? searchName , int index , int PageSize, string orderBy, string sortBy);

        Task<ResponseFeedbackModel> GetFeedbackId(string id);

        Task CreateFeedbackAsync(CreateFeedbackModel model);

        Task UpdateFeedbackAsync(UpdateFeedbackModel model);

        Task DeleteFeedbackAsync(string id);

    }
}
