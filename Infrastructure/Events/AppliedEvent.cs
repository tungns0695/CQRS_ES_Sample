using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Events
{
    public class AppliedEvent
    {
        [Key]
        public required string EventId { get; set; }
        
        public string ProjectorName { get; set; } = string.Empty;
        
        public DateTime AppliedAt { get; set; }
        
        public long Position { get; set; }
    }
}

