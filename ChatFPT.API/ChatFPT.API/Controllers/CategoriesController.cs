
using ChatFPT.API.MiddleWare.Attributes;
using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Feedback;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IRedisService _redisService;
        public CategoriesController(ICategoryService categoryService, IRedisService redisService)
        {
            _categoryService = categoryService;
            _redisService = redisService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
        {
            await _categoryService.CreateCategoryAsync(model);
            await _redisService.RemoveCacheResponseAsync("/api/categories");
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Tạo mới Category thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryModel model)
        {
            await _categoryService.UpdateCategoryAsync(model);
            await _redisService.RemoveCacheResponseAsync("/api/categories");
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Cập nhập Category thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            await _redisService.RemoveCacheResponseAsync("/api/categories");

            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Xóa Category thành công"));
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetCategory(string? searchName, int index = 1, int pageSize = 10)
        {
            var data = await _categoryService.GetCategoriesAsync(searchName, index, pageSize);
            return Ok(BaseResponseModel<IReadOnlyCollection<ResponseCategoryModel>>.OkDataResponse(data, "Lấy data thành công"));
        }

        [HttpGet("{id}")]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var data = await _categoryService.GetCategoryId(id);
            return Ok(BaseResponseModel<ResponseCategoryModel>.OkDataResponse(data, "Lấy data thành công"));
        }
    }
}
