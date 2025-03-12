

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.User;
using ChatFPT.Domain.Base;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatFPT.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtSettings _jwtSettings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper
            , IPasswordHasher passwordHasher
            , JwtSettings jwtSettings
            , JwtSecurityTokenHandler jwtSecurityTokenHandler
            , IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtSettings;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _contextAccessor = httpContextAccessor;
        }
        public async Task Delete(string id)
        {
            ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.Id == Guid.Parse(id) && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy User");

            user.DeletedTime = DateTime.Now;
            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task<UserInfoModel> GetUserInfo()
        {
            ApplicationUser? user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync

                (u => u.Id.ToString() == Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor));

            UserInfoModel model = _mapper.Map<UserInfoModel>(user);

            return model;
                
        }

        public async Task<LoginResponse> Login(LoginRequestModel model)
        {
            IQueryable<LoginQueryModel> queryModels = from user in _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                      join userRole in _unitOfWork.GetRepository<ApplicationUserRoles>().Entities on user.Id equals userRole.UserId
                                                      join role in _unitOfWork.GetRepository<ApplicationRole>().Entities on userRole.RoleId equals role.Id
                                                      where !user.DeletedTime.HasValue && user.UserName == model.UserName
                                                      select new LoginQueryModel()
                                                      {
                                                          User = user,
                                                          RoleName = role.Name,
                                                      };

            LoginQueryModel? result = await queryModels.FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Sai tên đăng nhập hoặc mật khẩu");

            if(!_passwordHasher.Verify(result.User!.PasswordHash!, model.Password))
            {
                 throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Sai tên đăng nhập hoặc mật khẩu");
            }

            return new LoginResponse()
            {
                TokenResponse = await Authentication.CreateToken(result.User!,result.RoleName!, _jwtSettings)
            };
        }

        public async Task<TokenResponse> RefreshToken(RefreshTokenRequestModel request)
        {           

            // 1. Xác thực refresh token
            ClaimsPrincipal? principal = ValidateRefreshToken(request.RefreshToken ?? "");

            // check nếu valid thành công sẽ trả về thông tin người dùng
            if (principal == null)
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstaints.UNAUTHORIZED, "Refresh token không hợp lệ.");
            }

            string? userId = principal.FindFirst("id")?.Value;
            string? userRole = principal.FindFirst(ClaimTypes.Role)?.Value;

            ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.Where(u => u.Id.ToString() == userId).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy người dùng");

            // 2. Tạo token mới
            TokenResponse response = await Authentication.CreateToken(user, userRole!, _jwtSettings, true);
            response.RefreshToken = string.Empty;
            response.User = null;

            return response;
        }

        

        public async Task LoginGoogle(string token)
        {
            
        }

        public async Task Register(RegisterRequestModel model)
        {
            model.CheckValid();
            if( await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.UserName == model.UserName && !u.DeletedTime.HasValue) != null)
            {
                 throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "UserName bị trùng");
            }
               
            var passwordHash = _passwordHasher.Hash(model.PasswordHash);
            
            ApplicationUser user = _mapper.Map<ApplicationUser>(model);
            user.Password = passwordHash;
            user.PasswordHash = passwordHash;
            user.CreatedTime = DateTime.Now;
            
            await _unitOfWork.GetRepository<ApplicationUser>().AddAsync(user);
            await _unitOfWork.SaveAsync();
            ApplicationUserRoles userRole = new ApplicationUserRoles
            {
                UserId = user.Id,
                RoleId = Guid.Parse(model.RoleId!)
            };
            await _unitOfWork.GetRepository<ApplicationUserRoles>().AddAsync(userRole);
            await _unitOfWork.SaveAsync();
        }

        private ClaimsPrincipal ValidateRefreshToken(string refreshToken)
        {
            // khởi tạo thông tin xác thực cho token  
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey ?? string.Empty)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            return _jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters, out _);
        }
    }      
}   

