using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedList<ResponseCategoryModel>> GetCategoriesAsync(string? searchName , int index , int PageSize);

        Task<ResponseCategoryModel> GetCategoryId(string id);

        Task CreateCategoryAsync(CreateCategoryModel model);

        Task UpdateCategoryAsync(UpdateCategoryModel model);

        Task DeleteCategoryAsync(string id);

    }
}
