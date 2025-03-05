using ChatFPT.Core.Base;
using ChatFPT.Core.Models.User;
using ChatFPT.Service.Interfaces;
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
        [HttpPost("LoginGoogle")]
        public async Task<IActionResult> LoginGoogle(string token)
        {
            await _authService.LoginGoogle(token);
            return Ok(BaseResponse<string>.OkDataResponse("OK"));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            await _authService.Register(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới tài khoản thành công"));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _authService.Delete(userId);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa tài khoản thành công"));
        }
    }
}
