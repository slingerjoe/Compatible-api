using System;

namespace CompatibleAPI.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        
        public Guid SenderProfileId { get; set; }
        public Profile SenderProfile { get; set; } = null!;
        
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
} 