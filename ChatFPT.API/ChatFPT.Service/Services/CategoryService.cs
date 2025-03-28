using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Pagination;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;

namespace ChatFPT.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = httpContextAccessor;
        }
        public async Task CreateCategoryAsync(CreateCategoryModel model)
        {
            model.ValidateFields();
            Category category = _mapper.Map<Category>(model);
            category.CreatedBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            category.CreatedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

        }

        public async Task DeleteCategoryAsync(string id)
        {
            Category category = await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeleteTime.HasValue)
                   ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy ID");

            category.DeleteTime = DateTime.Now;
            category.DeleteBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseCategoryModel>> GetCategoriesAsync(string? searchName, int index, int PageSize, string orderBy, string sortBy)
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

            if (!string.IsNullOrEmpty(orderBy))
            {
                string sortDirection = (sortBy?.ToLower() == "desc") ? "descending" : "ascending";
                try
                {
                    query = query.OrderBy($"{orderBy} {sortDirection}");
                }
                catch (ParseException)
                {
                    query = query.OrderBy("CategoryId");
                }
            }

            PaginatedList<ResponseCategoryModel> paginatedCate = await _unitOfWork.GetRepository<ResponseCategoryModel>().GetPagingAsync(query, index, PageSize);
            return paginatedCate;
        }

        public async Task<ResponseCategoryModel> GetCategoryId(string id)
        {
            Category cate = await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstaints.NOT_FOUND, "Không tìm thấy CategoryId");

            if (cate.DeleteTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status410Gone, ResponseCodeConstaints.GONE,
                     $"Answers đã bị xóa. Deleted by: {cate.DeleteBy}. Deleted date: {cate.DeleteTime}"
                );
            }
            
            return _mapper.Map<ResponseCategoryModel>(cate);

        }

        public async Task UpdateCategoryAsync(UpdateCategoryModel model)
        {
            model.ValidateFields();
            Category cate = await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(r => r.Id == model.Id && !r.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy CategoryId");

            _mapper.Map(model, cate);
            cate.LastUpdateTime = DateTime.Now;
            cate.LastUpdateBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            
                 await _unitOfWork.GetRepository<Category>().UpdateAsync(cate);
            await _unitOfWork.SaveAsync();
        }
    }
}
