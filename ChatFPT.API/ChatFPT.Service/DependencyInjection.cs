using ChatFPT.Service.Interfaces;
using ChatFPT.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatFPT.Service
{
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IGPTInterface, GptService>();
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
