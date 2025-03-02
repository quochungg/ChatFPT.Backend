
using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Category;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
        {
             await _categoryService.CreateCategoryAsync(model);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Tạo mới Category thành công"));
        }
    }
}
