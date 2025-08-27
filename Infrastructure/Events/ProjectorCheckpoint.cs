using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Events
{
    public class ProjectorCheckpoint
    {
        [Key]
        public string ProjectorName { get; set; } = string.Empty;
        
        public long LastPosition { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }
}

