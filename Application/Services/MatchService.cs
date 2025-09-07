using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompatibleAPI.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPhotoRepository _photoRepository;

        public MatchService(
            IProfileRepository profileRepository, 
            IMatchRepository matchRepository,
            IPhotoRepository photoRepository)
        {
            _profileRepository = profileRepository;
            _matchRepository = matchRepository;
            _photoRepository = photoRepository;
        }

        public async Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, int count = 20)
        {
            // Get potential matches with photos included
            var potentialMatches = await _matchRepository.GetPotentialMatchesAsync(profileId, count);

            // Get all profile IDs
            var profileIds = potentialMatches.Select(p => p.Id).ToList();

            // Get photos for all profiles
            var photos = await _photoRepository.GetPhotosByProfileIdsAsync(profileIds);

            // Group photos by profile ID
            var photosByProfileId = photos.GroupBy(p => p.ProfileId)
                                        .ToDictionary(g => g.Key, g => g.ToList());

            // Attach photos to each profile
            foreach (var profile in potentialMatches)
            {
                if (photosByProfileId.TryGetValue(profile.Id, out var profilePhotos))
                {
                    // Create a new list to avoid modifying the collection while enumerating
                    profile.Photos = new List<Photo>(profilePhotos);
                }
                else
                {
                    // Ensure Photos is never null
                    profile.Photos = new List<Photo>();
                }
            }

            return potentialMatches;
        }

        public async Task<Match> AcceptMatchAsync(Guid profileId, Guid matchedProfileId)
        {
            var existingMatch = await _matchRepository.GetMatchAsync(profileId, matchedProfileId);
            if (existingMatch != null)
            {
                existingMatch.IsAccepted = true;
                existingMatch.MatchedAt = DateTime.UtcNow;
                return await _matchRepository.UpdateMatchAsync(existingMatch);
            }

            var match = new Match
            {
                ProfileId = profileId,
                MatchedProfileId = matchedProfileId,
                IsAccepted = true,
                IsRejected = false,
                MatchedAt = DateTime.UtcNow
            };

            return await _matchRepository.CreateMatchAsync(match);
        }

        public async Task<Match> RejectMatchAsync(Guid profileId, Guid matchedProfileId)
        {
            var existingMatch = await _matchRepository.GetMatchAsync(profileId, matchedProfileId);
            if (existingMatch != null)
            {
                existingMatch.IsRejected = true;
                return await _matchRepository.UpdateMatchAsync(existingMatch);
            }

            var match = new Match
            {
                ProfileId = profileId,
                MatchedProfileId = matchedProfileId,
                IsAccepted = false,
                IsRejected = true
            };

            return await _matchRepository.CreateMatchAsync(match);
        }

        public async Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId)
        {
            return await _matchRepository.GetProfileMatchesAsync(profileId);
        }

        public async Task<IEnumerable<Match>> GetAcceptedMatchesAsync(Guid profileId)
        {
            Console.WriteLine($"GetAcceptedMatchesAsync called for profileId: {profileId}");
            var matches = await _matchRepository.GetProfileMatchesAsync(profileId, true);
            Console.WriteLine($"Found {matches.Count()} accepted matches for profileId: {profileId}");
            
            // Get all profile IDs from matches
            var profileIds = new List<Guid>();
            foreach (var match in matches)
            {
                if (match.ProfileId != profileId) profileIds.Add(match.ProfileId);
                if (match.MatchedProfileId != profileId) profileIds.Add(match.MatchedProfileId);
            }
            
            // Get profiles with photos
            var profiles = await _profileRepository.GetProfilesByIdsAsync(profileIds);
            var photos = await _photoRepository.GetPhotosByProfileIdsAsync(profileIds);
            
            // Group photos by profile ID
            var photosByProfileId = photos.GroupBy(p => p.ProfileId)
                                        .ToDictionary(g => g.Key, g => g.ToList());
            
            // Attach photos to profiles
            foreach (var profile in profiles)
            {
                if (photosByProfileId.TryGetValue(profile.Id, out var profilePhotos))
                {
                    profile.Photos = new List<Photo>(profilePhotos);
                }
                else
                {
                    profile.Photos = new List<Photo>();
                }
            }
            
            // Create a dictionary for quick profile lookup
            var profilesById = profiles.ToDictionary(p => p.Id);
            
            // Attach profiles to matches
            foreach (var match in matches)
            {
                if (profilesById.TryGetValue(match.ProfileId, out var profile))
                {
                    match.Profile = profile;
                }
                if (profilesById.TryGetValue(match.MatchedProfileId, out var matchedProfile))
                {
                    match.MatchedProfile = matchedProfile;
                }
            }
            
            return matches;
        }

        public async Task<Match?> GetMatchByIdAsync(Guid matchId)
        {
            return await _matchRepository.GetMatchByIdAsync(matchId);
        }
    }
} 