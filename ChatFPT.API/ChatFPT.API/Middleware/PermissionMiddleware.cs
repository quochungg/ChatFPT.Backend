using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatFPT.API.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleware> _logger;
        private readonly IEnumerable<string> _excludedUris;
        private readonly IMemoryCache _cache;

        public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger, IMemoryCache cache)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _excludedUris =
            [
                "/api/auth/login",
                "/api/role",
                "/api/auth/logingoogle",
                "/api/auth/register"
            ];
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {

            if (HasPermission(context, unitOfWork))
            {
                await _next(context);
            }
            else
            {
                await Authentication.HandleForbiddenRequest(context);
            }
        }

        private bool HasPermission(HttpContext context, IUnitOfWork unitOfWork)
        {
            string requestUri = context.Request.Path.Value!;
            string httpMethod = context.Request.Method;
            string[] segments = requestUri.Split('/');

            string featureUri = string.Join("/", segments.Take(segments.Length - 1));

            if (_excludedUris.Contains(requestUri) || !requestUri.StartsWith("/api/"))
            {
                return true;
            }
            //string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            // if (IsValidToken(token) != "access")
            //{
            //    throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstaints.FORBIDDEN, "Chỉ cho phép truy cập bằng Access Token.");
            //}
            //if (!requestUri.StartsWith("/api/auth/refresh-token"))
            //{
            //    if (IsTokenExpired(context) && IsValidToken(token) == "access")
            //    {
            //        throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstaints.UNAUTHORIZED, "Token không hợp lệ");
            //    }
            //}

            //try
            //{
            //    string userId = Authentication.GetUserIdFromHttpContext(context);
                
                
            //        IEnumerable<ApplicationRoleClaims>? roleClaims = unitOfWork.GetRepository<ApplicationUserRoles>()
            //        .Entities.Where(r => r.UserId.ToString() == userId)
            //        .Join(
            //            unitOfWork.GetRepository<ApplicationRole>().Entities,
            //            userRole => userRole.RoleId,
            //            role => role.Id,
            //            (userRole, role) => new { role.Id }
            //        )
            //        .SelectMany(role => unitOfWork.GetRepository<ApplicationRoleClaims>()
            //            .Entities.Where(rc => rc.RoleId == role.Id))
            //        .ToList();

                    

            //    if (roleClaims != null)
            //    {
            //        // Kiểm tra trong RoleClaims
            //        foreach (var roleClaim in roleClaims)
            //        {
            //            if (roleClaim.ClaimType!.Equals(httpMethod, StringComparison.OrdinalIgnoreCase)
            //                && requestUri.StartsWith(roleClaim.ClaimValue ?? "Unknown", StringComparison.OrdinalIgnoreCase))
            //            {
            //                return true;
            //            }
            //        }                
            //    }
            //}

            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error while checking permissions");
            //}

            return true;
        
    }


        private string IsValidToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);
            Claim tokenTypeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")
                ?? throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstaints.UNAUTHORIZED, "Không tìm thấy token type");

            return tokenTypeClaim.Value;

        }

        private bool IsTokenExpired(HttpContext context)
        {
            // Lấy token từ Header Authorization
            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (string.IsNullOrWhiteSpace(token))
            {
                return true;
            }

            try
            {
                // Giải mã và kiểm tra token
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                // Lấy thời gian hết hạn từ token
                DateTime expiration = jwtToken.ValidTo;

                if (expiration < DateTime.UtcNow)
                {
                    return true;
                }
            }
            catch (SecurityTokenException)
            {
                return true;
            }

            return false;
        }
    }
}
