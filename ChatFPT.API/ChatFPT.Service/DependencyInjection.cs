using ChatFPT.Service.Interfaces;
using ChatFPT.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace ChatFPT.Service
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper();
            services.AddServices(configuration);
            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddSingleton<JwtSecurityTokenHandler>();
            
        }
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddScoped<IGPTInterface, GptService>();
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IFeedBackService, FeedbackService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IRoleClaimService, RoleClaimService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IFcmService, FcmService>();
            services.AddScoped<IUploadDataService, UploadDataService>();
        }
        private static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
