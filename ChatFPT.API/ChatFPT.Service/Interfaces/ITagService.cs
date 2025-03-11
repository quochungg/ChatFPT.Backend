using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Models.Tag;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface ITagService
    {
        Task<PaginatedList<ResponseTagModel>> GetAllTag(string? searchName, int index, int PageSize);

        Task<ResponseTagModel> GetTagById(string id);

        Task CreateTag(CreateTagModel model);

        Task UpdateTag(UpdateTagModel model);

        Task DeleteTag(string id);


    }
}
