using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    /// <summary>
    /// Repository interface for managing photos
    /// </summary>
    public interface IPhotoRepository
    {
        /// <summary>
        /// Adds a new photo
        /// </summary>
        /// <param name="photo">The photo to add</param>
        /// <returns>The added photo</returns>
        Task<Photo> AddPhotoAsync(Photo photo);

        /// <summary>
        /// Gets all photos for a profile
        /// </summary>
        /// <param name="profileId">The profile ID</param>
        /// <returns>A collection of photos</returns>
        Task<IEnumerable<Photo>> GetPhotosByProfileIdAsync(Guid profileId);

        /// <summary>
        /// Gets photos for multiple profiles
        /// </summary>
        /// <param name="profileIds">The profile IDs</param>
        /// <returns>A collection of photos</returns>
        Task<IEnumerable<Photo>> GetPhotosByProfileIdsAsync(IEnumerable<Guid> profileIds);

        /// <summary>
        /// Deletes a photo
        /// </summary>
        /// <param name="photoId">The photo ID</param>
        /// <returns>True if the photo was deleted, false otherwise</returns>
        Task<bool> DeletePhotoAsync(Guid photoId);

        /// <summary>
        /// Sets a photo as the main photo for its profile
        /// </summary>
        /// <param name="photoId">The photo ID</param>
        /// <returns>True if the photo was set as main, false otherwise</returns>
        Task<bool> SetMainPhotoAsync(Guid photoId);
    }
} 