using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Hubs;

namespace CompatibleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessagesController(IMessageService messageService, IHubContext<MessageHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByMatchId(Guid matchId, [FromQuery] Guid profileId)
        {
            try
            {
                var messages = await _messageService.GetMessagesByMatchIdAsync(matchId, profileId);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("send")]
        public async Task<ActionResult<Message>> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var message = await _messageService.SendMessageAsync(request.MatchId, request.SenderProfileId, request.Content);
                return Ok(message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("unread/{profileId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUnreadMessages(Guid profileId)
        {
            var messages = await _messageService.GetUnreadMessagesAsync(profileId);
            return Ok(messages);
        }

        [HttpPost("read/{messageId}")]
        public async Task<ActionResult> MarkMessageAsRead(Guid messageId, [FromQuery] Guid profileId)
        {
            try
            {
                await _messageService.MarkMessageAsReadAsync(messageId, profileId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("unread-count/{profileId}")]
        public async Task<ActionResult<int>> GetUnreadMessageCount(Guid profileId)
        {
            var count = await _messageService.GetUnreadMessageCountAsync(profileId);
            return Ok(count);
        }

        [HttpPost("typing")]
        public async Task<ActionResult> SendTypingIndicator([FromBody] TypingIndicatorRequest request)
        {
            try
            {
                await _hubContext.Clients.Group($"match_{request.MatchId}").SendAsync("UserTyping", request.ProfileId, request.IsTyping);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class SendMessageRequest
    {
        public Guid MatchId { get; set; }
        public Guid SenderProfileId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class TypingIndicatorRequest
    {
        public Guid MatchId { get; set; }
        public Guid ProfileId { get; set; }
        public bool IsTyping { get; set; }
    }
} 