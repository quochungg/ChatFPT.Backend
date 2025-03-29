using ChatFPT.Core.Base;
using ChatFPT.Core.Models.AI;
using ChatFPT.Core.Models.Answer;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _uploadDataService;
        public AIController(IAIService uploadDataService)
        {
            _uploadDataService = uploadDataService;
        }
        [HttpPost("training")]
        public async Task<IActionResult> UploadTrainData(List<UploadDataModel> model)
        {
            var result = await _uploadDataService.UploadDataToPineconeAsync(model);
            return Ok(BaseResponse<string>.OkDataResponse(result,"tạo data thành công"));
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryData(string question)
        {
            var result = await _uploadDataService.QueryDataAsync(question);
            string id = "";
            return Ok(BaseResponse<string>.OkDataResponse(result, "truy vấn data thành công"));
        }
    }
}
