using ChatFPT.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFPT.Domain.Entities
{
    public class Feedback : AuditableEntity
    {
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? Rate { get; set; }
        public string? Note { get; set; }      
    }
}
