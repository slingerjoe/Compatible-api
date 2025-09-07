using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    /// <summary>
    /// Repository interface for profile data access
    /// </summary>
    public interface IProfileRepository
    {
        /// <summary>
        /// Gets all active profiles
        /// </summary>
        Task<IEnumerable<Profile>> GetAllAsync();

        /// <summary>
        /// Gets a profile by its ID
        /// </summary>
        Task<Profile?> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new profile
        /// </summary>
        Task<Profile> CreateAsync(Profile profile);

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        Task<Profile> UpdateAsync(Profile profile);

        /// <summary>
        /// Deletes (retires) a profile
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a profile exists
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Gets multiple profiles by their IDs
        /// </summary>
        Task<IEnumerable<Profile>> GetProfilesByIdsAsync(IEnumerable<Guid> ids);
    }
} 