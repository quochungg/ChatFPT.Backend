
using ChatFPT.Core.Models;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ChatFPT.API.MiddleWare.Attributes
{
    public class CacheAtribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToliveSeconds;
        public CacheAtribute(int timeToliveSeconds = 1000)
        {
            _timeToliveSeconds = timeToliveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheConfiguration = context.HttpContext.RequestServices.GetService<RedisConfiguration>();
            if (!cacheConfiguration.Enabled)
            {
                await next();
                return;
            }
            var cacheService = context.HttpContext.RequestServices.GetService<IRedisService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request );
            var cacheReponse = await cacheService.GetCacheResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheReponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cacheReponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }
            var executedContext = await next();
            if(executedContext.Result is OkObjectResult objectResult)
            {
                await cacheService.SetCacheResponseAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToliveSeconds));
            }
        }
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
