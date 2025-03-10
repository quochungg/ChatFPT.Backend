using ChatFPT.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFPT.Domain.Entities
{
    public class Question : AuditableEntity
    {
        [ForeignKey("ApplicationUser")]
        public Guid? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }
        
        public string? Content { get; set; }

        public bool IsResolve { get; set; }

    }
}
