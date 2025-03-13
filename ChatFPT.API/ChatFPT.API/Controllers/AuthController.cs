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

        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            UserInfoModel model =  await _authService.GetUserInfo();
            return Ok(BaseResponse<string>.OkDataResponse(model, "Lấy thông tin người dùng thành công"));
        }
                
        [HttpPost("google")]
        public async Task<IActionResult> LoginGoogle(string token)
        {
            TokenResponse tokenResponse = await _authService.LoginGoogle(token);
            return Ok(BaseResponse<string>.OkDataResponse(tokenResponse,"OK"));
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            LoginResponse response = await _authService.Login(model);
            return Ok(BaseResponse<string>.OkDataResponse(response,"Đăng nhập thành công"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestModel model)
        {
            TokenResponse response = await _authService.RefreshToken(model);
            return Ok(BaseResponse<string>.OkDataResponse(response, "Tạo mới token thành công"));
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            await _authService.Register(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới tài khoản thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _authService.Delete(id);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa tài khoản thành công"));
        }
    }
}
