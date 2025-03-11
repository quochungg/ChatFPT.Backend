using ChatFPT.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFPT.Domain.Entities
{
    public class Feedback : AuditableEntity
    {
        public string? AnswerId {  get; set; }
        public virtual Answer? Answer { get; set; }
        public int? Rate { get; set; }
      }
}
