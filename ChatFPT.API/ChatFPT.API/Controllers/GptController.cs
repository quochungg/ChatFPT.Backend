using ChatFPT.Application.Common;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/gpt")]
[ApiController]
public class GptController : ControllerBase
{
    private readonly IGPTInterface _gptService;

    public GptController(IGPTInterface gptService)
    {
        _gptService = gptService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskGpt([FromBody] GptRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        string response = await _gptService.GetGptResponse(request.Message);
        return Ok(new { response });
    }
}
