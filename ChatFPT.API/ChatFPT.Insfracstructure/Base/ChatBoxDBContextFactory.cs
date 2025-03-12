using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ChatFPT.Insfracstructure.Base
{
    public class ChatBoxDBContextFactory : IDesignTimeDbContextFactory<ChatBoxDBContext>
    {
       
        public ChatBoxDBContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ChatBoxDBContext>();

            builder.UseSqlServer("Server=tcp:chatfpt.database.windows.net,1433;Initial Catalog=ChatBoxFPT;Persist Security Info=False;User ID=chatfpt;Password=Admin@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            //builder.UseSqlServer("Server=.;Database=ChatBoxFPT;uid=sa;pwd=12345;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;");

            return new ChatBoxDBContext(builder.Options);
        }
    }
}
