using ChatFPT.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFPT.Domain.Entities
{
    public class StudentInfo : AuditableEntity
    {
        [ForeignKey("ApplicationUser")]
        public Guid? UserId { get; set; }
       
        public string? FullName { get; set; }
        public string? MSSV { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
