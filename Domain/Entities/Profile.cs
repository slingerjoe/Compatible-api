using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CompatibleAPI.Domain.Entities
{
    /// <summary>
    /// Represents a user profile in the dating application
    /// </summary>
    public class Profile : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user's name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's age
        /// </summary>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the user's bio
        /// </summary>
        [StringLength(500)]
        public string Bio { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of photos associated with the profile
        /// </summary>
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();

        // Navigation properties for matches
        public ICollection<Match> Matches { get; set; } = new List<Match>();
        public ICollection<Match> MatchedBy { get; set; } = new List<Match>();
    }
} 