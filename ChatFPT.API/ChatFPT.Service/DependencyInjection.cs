using ChatFPT.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChatFPT.Service
{
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IGPTInterface, GptService>();
        }
    }
}
