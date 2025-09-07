using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Services
{
    /// <summary>
    /// Service for handling profile-related business logic
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMatchService _matchService;

        /// <summary>
        /// Initializes a new instance of the ProfileService
        /// </summary>
        /// <param name="profileRepository">Repository for profile data access</param>
        /// <param name="matchService">Service for match operations</param>
        public ProfileService(
            IProfileRepository profileRepository,
            IMatchService matchService)
        {
            _profileRepository = profileRepository;
            _matchService = matchService;
        }

        /// <summary>
        /// Gets all profiles
        /// </summary>
        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            return await _profileRepository.GetAllAsync();
        }

        /// <summary>
        /// Gets a profile by ID
        /// </summary>
        public async Task<Profile?> GetProfileByIdAsync(Guid id)
        {
            return await _profileRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a new profile
        /// </summary>
        public async Task<Profile> CreateProfileAsync(Profile profile)
        {
            return await _profileRepository.CreateAsync(profile);
        }

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            return await _profileRepository.UpdateAsync(profile);
        }

        /// <summary>
        /// Deletes a profile
        /// </summary>
        public async Task DeleteProfileAsync(Guid id)
        {
            await _profileRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Gets potential matches for a user
        /// </summary>
        /// <param name="profileId">The ID of the profile to get matches for</param>
        /// <param name="count">The number of potential matches to retrieve</param>
        public async Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, int count = 20)
        {
            return await _matchService.GetPotentialMatchesAsync(profileId, count);
        }

        /// <summary>
        /// Likes a potential match
        /// </summary>
        /// <param name="profileId">The ID of the profile doing the liking</param>
        /// <param name="likedProfileId">The ID of the profile being liked</param>
        public async Task<bool> LikeMatchAsync(Guid profileId, Guid likedProfileId)
        {
            try
            {
                await _matchService.AcceptMatchAsync(profileId, likedProfileId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Dislikes a potential match
        /// </summary>
        /// <param name="profileId">The ID of the profile doing the disliking</param>
        /// <param name="dislikedProfileId">The ID of the profile being disliked</param>
        public async Task<bool> DislikeMatchAsync(Guid profileId, Guid dislikedProfileId)
        {
            try
            {
                await _matchService.RejectMatchAsync(profileId, dislikedProfileId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 