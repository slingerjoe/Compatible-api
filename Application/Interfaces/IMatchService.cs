using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, int count = 20);
        Task<Match> AcceptMatchAsync(Guid profileId, Guid matchedProfileId);
        Task<Match> RejectMatchAsync(Guid profileId, Guid matchedProfileId);
        Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId);
        Task<IEnumerable<Match>> GetAcceptedMatchesAsync(Guid profileId);
        Task<Match?> GetMatchByIdAsync(Guid matchId);
    }
} 