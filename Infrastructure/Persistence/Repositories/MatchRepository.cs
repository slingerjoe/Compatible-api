using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Application.Interfaces;

namespace CompatibleAPI.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for managing matches
    /// </summary>
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the MatchRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public MatchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, int count = 20)
        {
            // Get potential matches using EF Core
            return await _context.Profiles
                .Where(p => p.Id != profileId && p.RetiredAt == null)
                .Where(p => !_context.Matches.Any(m => 
                    (m.ProfileId == profileId && m.MatchedProfileId == p.Id) ||
                    (m.MatchedProfileId == profileId && m.ProfileId == p.Id)))
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Match?> GetMatchAsync(Guid profileId, Guid matchedProfileId)
        {
            return await _context.Matches
                .FirstOrDefaultAsync(m => 
                    m.ProfileId == profileId && 
                    m.MatchedProfileId == matchedProfileId && 
                    m.RetiredAt == null);
        }

        /// <inheritdoc />
        public async Task<Match?> GetMatchByIdAsync(Guid matchId)
        {
            return await _context.Matches
                .Include(m => m.Profile)
                .Include(m => m.MatchedProfile)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.RetiredAt == null);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId)
        {
            return await _context.Matches
                .Where(m => (m.ProfileId == profileId || m.MatchedProfileId == profileId) && 
                           m.RetiredAt == null)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Match> CreateMatchAsync(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            return match;
        }

        /// <inheritdoc />
        public async Task<Match> UpdateMatchAsync(Match match)
        {
            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
            return match;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId, bool isAccepted)
        {
            Console.WriteLine($"GetProfileMatchesAsync called for profileId: {profileId}, isAccepted: {isAccepted}");
            
            var allMatches = await _context.Matches.ToListAsync();
            Console.WriteLine($"Total matches in database: {allMatches.Count}");
            
            foreach (var match in allMatches)
            {
                Console.WriteLine($"Match: ProfileId={match.ProfileId}, MatchedProfileId={match.MatchedProfileId}, IsAccepted={match.IsAccepted}, RetiredAt={match.RetiredAt}");
            }
            
            var filteredMatches = await _context.Matches
                .Where(m => (m.ProfileId == profileId || m.MatchedProfileId == profileId) && 
                           m.RetiredAt == null &&
                           m.IsAccepted == isAccepted)
                .ToListAsync();
                
            Console.WriteLine($"Filtered matches found: {filteredMatches.Count}");
            return filteredMatches;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(Guid profileId, Guid matchedProfileId)
        {
            return await _context.Matches
                .AnyAsync(m => 
                    m.ProfileId == profileId && 
                    m.MatchedProfileId == matchedProfileId && 
                    m.RetiredAt == null);
        }
    }
} 