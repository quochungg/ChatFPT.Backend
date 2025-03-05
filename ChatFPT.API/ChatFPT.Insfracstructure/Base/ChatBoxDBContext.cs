using ChatFPT.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace ChatFPT.Insfracstructure.Base
{
    public class ChatBoxDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ChatBoxDBContext(DbContextOptions<ChatBoxDBContext> options) : base(options) {

        }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ApplicationUserRoles> ApplicationUserRoles { get; set; }
        public virtual DbSet<ApplicationRole> ApplicationRole {  get; set; }

        public virtual DbSet<Answer> Answers { get; set; }

        public virtual DbSet<Feedback> Feedbacks { get; set; }

        public virtual DbSet<Question> Questions { get; set; }

        public virtual DbSet<QuestionTag> QuestionTags { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đánh dấu QuestionTag là keyless entity
            modelBuilder.Entity<QuestionTag>()
                .HasNoKey();
        }
    }

}
    

