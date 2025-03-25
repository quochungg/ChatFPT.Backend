
using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Answer;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswersController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnswer(string? searchName, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseAnswerModel> list = await _answerService.GetAllAnswers(searchName, index, pageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseAnswerModel>>.OkDataResponse(list, "Lấy danh sách thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswer(CreateAnswerModel model)
        {
            await _answerService.CreateAnswer(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAnswer(UpdateAnswerModel model)
        {
            await _answerService.UpdateAnswer(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Cập nhật thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(string? id)
        {
            await _answerService.DeleteAnswer(id);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa thành công"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerById(string id)
        {
            var data = await _answerService.GetAnswerById(id);
            return Ok(BaseResponse<ResponseAnswerModel>.OkDataResponse(data, "Lấy data thành công"));
        }

        [HttpGet("question/{id}")]
        public async Task<IActionResult> GetAnswerByQuestionId(string? id)
        {
            ResponseAnswerModel model = await _answerService.GetAnswerByQuestionId(id);

            return Ok(BaseResponse<ResponseAnswerModel>.OkDataResponse(model, "Lấy data thành công"));
        }
    }
}