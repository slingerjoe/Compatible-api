using System;

namespace CompatibleAPI.Domain.Entities
{
    public class Match : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        
        public Guid MatchedProfileId { get; set; }
        public Profile MatchedProfile { get; set; } = null!;
        
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime? MatchedAt { get; set; }

        /// <summary>
        /// Gets or sets the compatibility score between the two profiles
        /// </summary>
        public int Compatibility { get; set; }
    }
} 