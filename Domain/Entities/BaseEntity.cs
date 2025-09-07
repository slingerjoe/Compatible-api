using System;
using System.ComponentModel.DataAnnotations;

namespace CompatibleAPI.Domain.Entities
{
    /// <summary>
    /// Base class for all entities with common audit fields
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity (Primary Key)
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was retired (soft deleted)
        /// </summary>
        public DateTime? RetiredAt { get; set; }
    }
} 