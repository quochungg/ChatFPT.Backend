

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
app.UseAuthentication();

app.MapControllers();

app.Run();
