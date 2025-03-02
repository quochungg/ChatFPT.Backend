using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Pagination;

namespace ChatFPT.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedList<CategoryModel>> GetCategoriesAsync(string? searchName , int index , int PageSize);

        Task<CategoryModel> GetCategoryId(int id);
        Task CreateCategoryAsync(CreateCategoryModel model);

        Task UpdateCategoryAsync(CategoryModel model);

        Task DeleteCategoryAsync(int id);

    }
}
