using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    /// <summary>
    /// Interface for profile-related business operations
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Gets all profiles
        /// </summary>
        Task<IEnumerable<Profile>> GetAllProfilesAsync();

        /// <summary>
        /// Gets a profile by ID
        /// </summary>
        /// <param name="id">The ID of the profile to retrieve</param>
        Task<Profile?> GetProfileByIdAsync(Guid id);

        /// <summary>
        /// Creates a new profile
        /// </summary>
        /// <param name="profile">The profile information to create</param>
        Task<Profile> CreateProfileAsync(Profile profile);

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        /// <param name="profile">The updated profile information</param>
        Task<Profile> UpdateProfileAsync(Profile profile);

        /// <summary>
        /// Deletes a profile
        /// </summary>
        /// <param name="id">The ID of the profile to delete</param>
        Task DeleteProfileAsync(Guid id);

        /// <summary>
        /// Gets potential matches for a user
        /// </summary>
        /// <param name="profileId">The ID of the profile to get matches for</param>
        /// <param name="count">The number of potential matches to retrieve</param>
        Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, int count = 20);

        /// <summary>
        /// Likes a potential match
        /// </summary>
        /// <param name="profileId">The ID of the profile doing the liking</param>
        /// <param name="likedProfileId">The ID of the profile being liked</param>
        Task<bool> LikeMatchAsync(Guid profileId, Guid likedProfileId);

        /// <summary>
        /// Dislikes a potential match
        /// </summary>
        /// <param name="profileId">The ID of the profile doing the disliking</param>
        /// <param name="dislikedProfileId">The ID of the profile being disliked</param>
        Task<bool> DislikeMatchAsync(Guid profileId, Guid dislikedProfileId);
    }
} 