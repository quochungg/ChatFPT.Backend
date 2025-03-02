using ChatFPT.Core.Base;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> LoginGoogle(string token)
        {
            await _authService.LoginGoogle(token);
            return Ok(BaseResponse<string>.OkDataResponse("OK"));
        }
    }
}
