
using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Feedback;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
        {
            await _categoryService.CreateCategoryAsync(model);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Tạo mới Category thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryModel model)
        {
            await _categoryService.UpdateCategoryAsync(model);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Cập nhập Category thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Xóa Category thành công"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(string? searchName, int index = 1, int pageSize = 10, string orderBy = "CategoryId", string sortBy = "DESC")
        {
            var data = await _categoryService.GetCategoriesAsync(searchName, index, pageSize,  orderBy,  sortBy);
            return Ok(BaseResponseModel<IReadOnlyCollection<ResponseCategoryModel>>.OkDataResponse(data, "Lấy data thành công"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var data = await _categoryService.GetCategoryId(id);
            return Ok(BaseResponseModel<ResponseCategoryModel>.OkDataResponse(data, "Lấy data thành công"));
        }
    }
}
