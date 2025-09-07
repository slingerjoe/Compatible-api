using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Application.Interfaces;

namespace CompatibleAPI.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for managing photos
    /// </summary>
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the PhotoRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public PhotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Photo>> GetPhotosByProfileIdAsync(Guid profileId)
        {
            return await _context.Photos
                .Where(p => p.ProfileId == profileId && p.RetiredAt == null)
                .OrderByDescending(p => p.IsMain)
                .ThenBy(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Photo>> GetPhotosByProfileIdsAsync(IEnumerable<Guid> profileIds)
        {
            return await _context.Photos
                .Where(p => profileIds.Contains(p.ProfileId) && p.RetiredAt == null)
                .OrderByDescending(p => p.IsMain)
                .ThenBy(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> DeletePhotoAsync(Guid photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null || photo.RetiredAt != null)
            {
                return false;
            }

            photo.RetiredAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> SetMainPhotoAsync(Guid photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null || photo.RetiredAt != null)
            {
                return false;
            }

            // Unset any existing main photos
            var existingMainPhotos = await _context.Photos
                .Where(p => p.ProfileId == photo.ProfileId && p.IsMain && p.RetiredAt == null)
                .ToListAsync();

            foreach (var mainPhoto in existingMainPhotos)
            {
                mainPhoto.IsMain = false;
            }

            // Set this photo as main
            photo.IsMain = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 