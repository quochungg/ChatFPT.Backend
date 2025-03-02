

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Pagination;
using ChatFPT.Domain.Entities;

using ChatFPT.Service.Interfaces;

namespace ChatFPT.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork) {
            _mapper = mapper;      
            _unitOfWork = unitOfWork;
        }
        public async Task CreateCategoryAsync(CreateCategoryModel model)
        {
            model.checkValid();
            Category category = _mapper.Map<Category>(model);
            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();
            
        }

        public Task DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<CategoryModel>> GetCategoriesAsync(string? searchName, int index, int PageSize)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryModel> GetCategoryId(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(CategoryModel model)
        {
            throw new NotImplementedException();
        }
    }
}
