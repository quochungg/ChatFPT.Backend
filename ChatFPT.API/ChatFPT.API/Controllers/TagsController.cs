using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Tag;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags(string? searchName, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseTagModel> list = await _tagService.GetAllTag(searchName, index, pageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseTagModel>>.OkDataResponse(list, "Lấy danh sách thành công"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(string id)
        {
            ResponseTagModel model = await _tagService.GetTagById(id);
            return Ok(BaseResponse<string>.OkDataResponse(model, "Lấy Tag thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag(CreateTagModel model)
        {
            await _tagService.CreateTag(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới tag thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTag(UpdateTagModel model)
        {
            await _tagService.UpdateTag(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Cập nhật Tag thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(string id)
        {
            await _tagService.DeleteTag(id);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa Tag thành công"));
        }
    }
}
