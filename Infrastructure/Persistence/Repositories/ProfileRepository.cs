using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Application.Interfaces;

namespace CompatibleAPI.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for managing profiles
    /// </summary>
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the ProfileRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public ProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Profile?> GetByIdAsync(Guid id)
        {
            return await _context.Profiles
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id && p.RetiredAt == null);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Profile>> GetAllAsync()
        {
            return await _context.Profiles
                .Include(p => p.Photos)
                .Where(p => p.RetiredAt == null)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Profile> CreateAsync(Profile profile)
        {
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        /// <inheritdoc />
        public async Task<Profile> UpdateAsync(Profile profile)
        {
            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile != null && profile.RetiredAt == null)
            {
                profile.RetiredAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Profiles
                .AnyAsync(p => p.Id == id && p.RetiredAt == null);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Profile>> GetProfilesByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Profiles
                .Include(p => p.Photos)
                .Where(p => ids.Contains(p.Id) && p.RetiredAt == null)
                .ToListAsync();
        }
    }
} 