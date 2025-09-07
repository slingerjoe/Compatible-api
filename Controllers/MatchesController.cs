using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet("potential/{profileId}")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetPotentialMatches(Guid profileId)
        {
            var matches = await _matchService.GetPotentialMatchesAsync(profileId);
            return Ok(matches);
        }

        [HttpPost("accept/{profileId}/{matchedProfileId}")]
        public async Task<ActionResult<Match>> AcceptMatch(Guid profileId, Guid matchedProfileId)
        {
            var match = await _matchService.AcceptMatchAsync(profileId, matchedProfileId);
            return Ok(match);
        }

        [HttpPost("reject/{profileId}/{matchedProfileId}")]
        public async Task<ActionResult<Match>> RejectMatch(Guid profileId, Guid matchedProfileId)
        {
            var match = await _matchService.RejectMatchAsync(profileId, matchedProfileId);
            return Ok(match);
        }

        [HttpGet("profile/{profileId}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetProfileMatches(Guid profileId)
        {
            var matches = await _matchService.GetProfileMatchesAsync(profileId);
            return Ok(matches);
        }

        [HttpGet("accepted/{profileId}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetAcceptedMatches(Guid profileId)
        {
            var matches = await _matchService.GetAcceptedMatchesAsync(profileId);
            return Ok(matches);
        }
    }
} 