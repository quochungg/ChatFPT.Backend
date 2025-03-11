
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
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedBackService _feedbackService;
        public FeedbackController(IFeedBackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateFeedback(CreateFeedbackModel model)
        {
            await _feedbackService.CreateFeedbackAsync(model);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Tạo mới Feedback thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeedback(UpdateFeedbackModel model)
        {
            await _feedbackService.UpdateFeedbackAsync(model);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Cập nhập Feedback thành công"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFeedback(string Id)
        {
            await _feedbackService.DeleteFeedbackAsync(Id);
            return Ok(BaseResponseModel<string>.OkMessageResponseModel("Xóa Category thành công"));
        }

        [HttpGet]
        public async Task<IActionResult> GetFeedback(string? CreatedById, int index = 1, int pageSize = 10)
        {
            var data = await _feedbackService.GetFeedbacksAsync(CreatedById, index, pageSize);
            return Ok(BaseResponseModel<IReadOnlyCollection<ResponseFeedbackModel>>.OkDataResponse(data, "Lấy data thành công"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var data = await _feedbackService.GetFeedbackId(id);
            return Ok(BaseResponseModel<ResponseFeedbackModel>.OkDataResponse(data, "Lấy data thành công"));
        }
    }
}
