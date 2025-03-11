

using ChatFPT.API.DI;
using ChatFPT.API.Middleware;
using ChatFPT.API.MiddleWare;
using ChatFPT.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure the HTTP request pipeline.

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPT API v1");
        c.RoutePrefix = string.Empty;
    });
//}
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<PermissionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("CorsPolicy");
app.UseAuthentication();

app.MapControllers();

app.Run();
