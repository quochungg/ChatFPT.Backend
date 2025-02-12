using ChatFPT.Domain.Base;

namespace ChatFPT.Domain.Entities
{
    public class StudentInfo : AuditableEntity
    {

        public string? UserId { get; set; }
       
        public string? FullName { get; set; }
        public string? MSSV { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
