using ChatFPT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Insfracstructure.Base
{
    public class ChatBoxDBContext : DbContext
    {
        public ChatBoxDBContext(DbContextOptions<ChatBoxDBContext> options) : base(options) {
            
        }
        public DbSet<Category> Categories { get; set; }
    }
}
