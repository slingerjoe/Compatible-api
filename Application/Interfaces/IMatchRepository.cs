using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    public interface IMatchRepository
    {
        Task<Match?> GetMatchAsync(Guid profileId, Guid matchedProfileId);
        Task<Match?> GetMatchByIdAsync(Guid matchId);
        Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId);
        Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId, bool isAccepted);
        Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, int count = 20);
        Task<Match> CreateMatchAsync(Match match);
        Task<Match> UpdateMatchAsync(Match match);
        Task<bool> ExistsAsync(Guid profileId, Guid matchedProfileId);
    }
} 