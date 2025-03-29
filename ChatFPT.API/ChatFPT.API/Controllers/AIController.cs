using ChatFPT.Core.Models.AI;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(result);
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryData(string question)
        {
            var result = await _uploadDataService.QueryDataAsync(question);
            return Ok(result);
        }
    }
}
