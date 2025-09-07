using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Application.Interfaces;

namespace CompatibleAPI.Controllers
{
    /// <summary>
    /// Controller for managing user profiles and dating functionality
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfilesController : ControllerBase
    {
        /// <summary>
        /// Service for handling profile-related operations
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        /// Initializes a new instance of the ProfilesController
        /// </summary>
        /// <param name="profileService">Service for profile operations</param>
        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Retrieves all profiles from the database
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Profile>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            ActionResult<IEnumerable<Profile>> result;
            try
            {
                IEnumerable<Profile> profiles = await _profileService.GetAllProfilesAsync();
                result = Ok(profiles);
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to retrieve profiles", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Retrieves a specific profile by ID
        /// </summary>
        /// <param name="id">The ID of the profile to retrieve</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Profile>> GetProfile(Guid id)
        {
            ActionResult<Profile> result;
            try
            {
                Profile? profile = await _profileService.GetProfileByIdAsync(id);
                result = profile == null ? NotFound() : Ok(profile);
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to retrieve profile", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        /// <param name="id">The ID of the profile to update</param>
        /// <param name="profile">The updated profile information</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile(Guid id, Profile profile)
        {
            IActionResult result;
            try
            {
                if (id != profile.Id)
                {
                    result = BadRequest(new { error = "Profile ID mismatch" });
                }
                else
                {
                    Profile updatedProfile = await _profileService.UpdateProfileAsync(profile);
                    result = Ok(updatedProfile);
                }
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to update profile", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Creates a new profile
        /// </summary>
        /// <param name="profile">The profile information to create</param>
        [HttpPost]
        [ProducesResponseType(typeof(Profile), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Profile>> CreateProfile(Profile profile)
        {
            ActionResult<Profile> result;
            try
            {
                Profile createdProfile = await _profileService.CreateProfileAsync(profile);
                result = CreatedAtAction(nameof(GetProfile), new { id = createdProfile.Id }, createdProfile);
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to create profile", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Deletes a profile
        /// </summary>
        /// <param name="id">The ID of the profile to delete</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            IActionResult result;
            try
            {
                Profile? profile = await _profileService.GetProfileByIdAsync(id);
                if (profile == null)
                {
                    result = NotFound();
                }
                else
                {
                    await _profileService.DeleteProfileAsync(id);
                    result = NoContent();
                }
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to delete profile", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Retrieves potential matches for the current user
        /// </summary>
        /// <param name="profileId">The ID of the profile to get matches for</param>
        /// <param name="count">The number of potential matches to retrieve</param>
        [HttpGet("potential-matches")]
        [ProducesResponseType(typeof(IEnumerable<Profile>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Profile>>> GetPotentialMatches(
            [FromQuery] Guid profileId,
            [FromQuery] int count = 20)
        {
            ActionResult<IEnumerable<Profile>> result;
            try
            {
                IEnumerable<Profile> matches = await _profileService.GetPotentialMatchesAsync(profileId, count);
                result = Ok(matches);
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to get potential matches", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Likes a potential match
        /// </summary>
        /// <param name="profileId">The ID of the profile doing the liking</param>
        /// <param name="likedProfileId">The ID of the profile being liked</param>
        [HttpPost("like/{profileId}/{likedProfileId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LikeMatch(Guid profileId, Guid likedProfileId)
        {
            ActionResult result;
            try
            {
                bool success = await _profileService.LikeMatchAsync(profileId, likedProfileId);
                result = success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to like match", message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Dislikes a potential match
        /// </summary>
        /// <param name="profileId">The ID of the profile doing the disliking</param>
        /// <param name="dislikedProfileId">The ID of the profile being disliked</param>
        [HttpPost("dislike/{profileId}/{dislikedProfileId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DislikeMatch(Guid profileId, Guid dislikedProfileId)
        {
            ActionResult result;
            try
            {
                bool success = await _profileService.DislikeMatchAsync(profileId, dislikedProfileId);
                result = success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { error = "Failed to dislike match", message = ex.Message });
            }
            return result;
        }
    }
} 