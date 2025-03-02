

namespace ChatFPT.Domain.Base
{
    public abstract class AuditableEntity
    {
        protected AuditableEntity()
        {
            Id = Guid.NewGuid().ToString("N");
            CreatedTime = LastUpdateTime = DateTimeOffset.UtcNow;
        }
        public string Id { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastUpdateTime { get; set; }
        public string? LastUpdateBy { get; set; }

        public DateTimeOffset? DeleteTime { get; set; }
        public string? DeleteBy { get; set; }
    }
        
}
