using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IUploadDataService _uploadDataService;
        public AIController(IUploadDataService uploadDataService)
        {
            _uploadDataService = uploadDataService;
        }
        [HttpPost("training")]
        public async Task<IActionResult> UploadTrainData(List<string> data)
        {
            var result = await _uploadDataService.UploadDataToPineconeAsync(data);
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
