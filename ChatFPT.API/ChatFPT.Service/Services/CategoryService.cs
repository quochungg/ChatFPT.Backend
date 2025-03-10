

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;
using ChatFPT.Core.Utils;
using ChatFPT.Domain.Entities;

using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateCategoryAsync(CreateCategoryModel model)
        {
            model.ValidateFields();
            Category category = _mapper.Map<Category>(model);
            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

        }

        public async Task DeleteCategoryAsync(string id)
        {
            Category category = await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeleteTime.HasValue)
                   ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy ID");

            category.DeleteTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseCategoryModel>> GetCategoriesAsync(string? searchName, int index, int PageSize)
        {
            IQueryable<ResponseCategoryModel> query = from category in
                                              _unitOfWork.GetRepository<Category>().Entities
                                                      where !category.DeleteTime.HasValue
                                                      select new ResponseCategoryModel
                                                      {
                                                          CategoryId = category.Id,
                                                          CategoryName = category.CategoryName,
                                                          Description = category.Description,

                                                          CreatedTime = category.CreatedTime


                                                      };

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(s => s.CategoryName!.Contains(searchName));
            }

            PaginatedList<ResponseCategoryModel> paginatedCate = await _unitOfWork.GetRepository<ResponseCategoryModel>().GetPagingAsync(query, index, PageSize);
            return paginatedCate;
        }

        public async Task<ResponseCategoryModel> GetCategoryId(string id)
        {
            Category cate = await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy CategoryId");
            return new ResponseCategoryModel
            {
                CategoryId = cate.Id,
                CategoryName = cate.CategoryName,
                Description = cate.Description,
                CreatedTime = cate.CreatedTime,
            };
        }

        public async Task UpdateCategoryAsync(UpdateCategoryModel model)
        {
            model.ValidateFields();
            Category cate = await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(r => r.Id == model.Id && !r.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy CategoryId");

            _mapper.Map(model, cate);
            cate.LastUpdateTime = DateTime.UtcNow;
            cate.LastUpdateBy = model.UpdateBy;
                 await _unitOfWork.GetRepository<Category>().UpdateAsync(cate);
            await _unitOfWork.SaveAsync();
        }
    }
}
