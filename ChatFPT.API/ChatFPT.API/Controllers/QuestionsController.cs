using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Question;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestion(string? searchName, int index = 1, int PageSize = 10, string orderBy = "Id", string sortBy = "DESC")
        {
            PaginatedList<ResponseQuestionModel> paginatedList = await _questionService.GetAllQuestion(searchName, index, PageSize, orderBy, sortBy);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseQuestionModel>>.OkDataResponse(paginatedList, "Lấy danh sách thành công"));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(string id)
        {
            ResponseQuestionModel model = await _questionService.GetQuestionById(id);
            return Ok(BaseResponse<string>.OkDataResponse(model, "Lấy Question thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(RequestQuestionModel model)
        {
            await _questionService.CreateQuestion(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới Question thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuestion(UpdateQuestionModel model)
        {
            await _questionService.UpdateQuestion(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Cập nhật Question thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            await _questionService.DeleteQuestion(id);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa Question thành công"));
        }
    }
}
