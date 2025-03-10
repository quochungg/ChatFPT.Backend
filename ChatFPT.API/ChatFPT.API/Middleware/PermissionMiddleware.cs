using ChatFPT.Application.Interface;
using ChatFPT.Service.Insfracstructure;
using Microsoft.Extensions.Caching.Memory;

namespace ChatFPT.API.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleware> _logger;
        private readonly IEnumerable<string> _excludedUris;
        private readonly IMemoryCache _cache;
        private string? _cacheKey;

        public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger, IMemoryCache cache)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _excludedUris =
            [
                "/api/auth/login",
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
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error while checking permissions");
            //}

            return true;
        }
    }
}
