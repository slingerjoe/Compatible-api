using System;
using System.ComponentModel.DataAnnotations;

namespace CompatibleAPI.Domain.Entities
{
    /// <summary>
    /// Represents a photo associated with a profile
    /// </summary>
    public class Photo : BaseEntity
    {
        /// <summary>
        /// Gets or sets the URL of the photo
        /// </summary>
        [Required]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the profile this photo belongs to
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// Gets or sets the profile this photo belongs to
        /// </summary>
        public Profile? Profile { get; set; }

        /// <summary>
        /// Gets or sets whether this is the main photo
        /// </summary>
        public bool IsMain { get; set; }
    }
} 