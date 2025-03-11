using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Question;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        [Route("GetAllQuestions")]
        public async Task<IActionResult> GetAllTag(string? searchName, int index = 1, int PageSize = 10)
        {
            PaginatedList<ResponseQuestionModel> paginatedList = await _questionService.GetAllQuestion(searchName, index, PageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseQuestionModel>>.OkDataResponse(paginatedList, "Lấy danh sách thành công"));

        }

        [HttpGet]
        [Route("GetQuestionById")]
        public async Task<IActionResult> GetQuestionById(string? tagId)
        {
            ResponseQuestionModel model = await _questionService.GetQuestionById(tagId!);
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

        [HttpDelete]
        public async Task<IActionResult> DeleteQuestion(string? questionId)
        {
            await _questionService.DeleteQuestion(questionId!);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa Question thành công"));
        }
    }
}
