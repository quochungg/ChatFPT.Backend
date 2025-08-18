using ChatFPT.Application.Interface;
using ChatFPT.Application.Repositories.ChatFPT.Infrastructure.Repositories;
using ChatFPT.Core.Models;
using ChatFPT.Core.Utils;
using ChatFPT.Domain.Base;
using ChatFPT.Insfracstructure.Base;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Reflection;
using System.Text;

namespace ChatFPT.API.DI
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigSwagger();
            services.ConfigCors();
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddEndpointsApiExplorer();
            services.AddUnitOfWork();
            services.JwtSettingsConfig(configuration);
            services.AddAuthenJwt();
            services.AddFirebaseServices(configuration);
            //services.ConfigRedis(configuration);
            //services.AddSignalR();
            services.AddRedis(configuration);

        }
        public static void JwtSettingsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(option =>
            {
                JwtSettings jwtSettings = new JwtSettings
                {
                    SecretKey = configuration.GetValue<string>("JwtSettings:SecretKey"),
                    Issuer = configuration.GetValue<string>("JwtSettings:Issuer"),
                    Audience = configuration.GetValue<string>("JwtSettings:Audience"),
                    AccessTokenExpirationMinutes = configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes"),
                    RefreshTokenExpirationDays = configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays")
                };
                jwtSettings.IsValid();
                return jwtSettings;
            });
        }
        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("*")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
        }

        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            RedisConfiguration redisSetting = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisSetting);

            services.AddSingleton(redisSetting);

            if (!redisSetting.Enabled)
                return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSetting.ConnectionString));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSetting.ConnectionString;
            });

        }
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        public static void ConfigCorsSignalR(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7016")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });
        }
        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void ConfigSwagger(this IServiceCollection services)
        {
            // config swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "API"

                });

                // Tùy chỉnh Swagger để hỗ trợ TimeOnly dưới dạng chuỗi
                c.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time",
                    Example = new OpenApiString("00:00:00")
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                //// Thêm JWT Bearer Token vào Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header sử dụng scheme Bearer.",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Name = "Authorization",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                c.OrderActionsBy((apiDesc) =>
                {
                    if (apiDesc.HttpMethod == "POST") return "3";
                    if (apiDesc.HttpMethod == "GET") return "1";
                    if (apiDesc.HttpMethod == "PUT") return "2";
                    if (apiDesc.HttpMethod == "DELETE") return "4";
                    return "5";
                });
            });
        }
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChatBoxDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultSQLConnection"));
            });
        }
        public static void AddAuthenJwt(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var jwtSettings = serviceProvider.GetRequiredService<JwtSettings>();
            services.AddAuthentication(e =>
            {
                e.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                e.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(e =>
            {
                e.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!))

                };
                e.SaveToken = true;
                e.RequireHttpsMetadata = true;
            });
        }

        public static void AddFirebaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Firebase Auth Configuration
            var firebaseAuthConfig = configuration.GetSection("Firebase").Get<Dictionary<string, object>>();
            if (firebaseAuthConfig != null)
            {
                var authCredential = GoogleCredential.FromJson(JsonConvert.SerializeObject(firebaseAuthConfig));

                FirebaseApp authApp;
                if (FirebaseApp.GetInstance("AuthApp") == null)
                {
                    authApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = authCredential
                    }, "AuthApp");
                }
                else
                {
                    authApp = FirebaseApp.GetInstance("AuthApp");
                }

                services.AddSingleton(authCredential);
                services.AddSingleton(authApp);
                services.AddSingleton(provider => FirebaseAuth.GetAuth(authApp));
            }

            // Firebase Push Notification Configuration
            var firebasePushConfig = configuration.GetSection("FirebasePushNotification").Get<Dictionary<string, object>>();
            if (firebasePushConfig != null)
            {
                var pushCredential = GoogleCredential.FromJson(JsonConvert.SerializeObject(firebasePushConfig));

                FirebaseApp pushApp;
                if (FirebaseApp.GetInstance("PushApp") == null)
                {
                    pushApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = pushCredential
                    }, "PushApp");
                }
                else
                {
                    pushApp = FirebaseApp.GetInstance("PushApp");
                }

                services.AddSingleton(pushCredential);
                services.AddSingleton(pushApp);
                services.AddSingleton(provider => FirebaseMessaging.GetMessaging(pushApp));
            }

            services.AddSingleton<FirebaseAuthHelper>();

        }
    }
}
