using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChatFPT.Insfracstructure.Base
{
    public class ChatBoxDBContextFactory : IDesignTimeDbContextFactory<ChatBoxDBContext>
    {
        public ChatBoxDBContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) 
                .Build();

            string? connectionString = configuration.GetConnectionString("DefaultSQLConnection");

            var builder = new DbContextOptionsBuilder<ChatBoxDBContext>();
            builder.UseSqlServer(connectionString);

            return new ChatBoxDBContext(builder.Options);
        }
    }
}