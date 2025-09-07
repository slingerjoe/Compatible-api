using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using CompatibleAPI.Hubs;

namespace CompatibleAPI.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMatchService _matchService;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageService(IMessageRepository messageRepository, IMatchService matchService, IHubContext<MessageHub> hubContext)
        {
            _messageRepository = messageRepository;
            _matchService = matchService;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<Message>> GetMessagesByMatchIdAsync(Guid matchId, Guid profileId)
        {
            // Verify the user has access to this match
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null || (match.ProfileId != profileId && match.MatchedProfileId != profileId))
            {
                throw new UnauthorizedAccessException("You don't have access to this conversation");
            }

            return await _messageRepository.GetMessagesByMatchIdAsync(matchId);
        }

        public async Task<Message> SendMessageAsync(Guid matchId, Guid senderProfileId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Message content cannot be empty");
            }

            // Verify the user has access to this match
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null || (match.ProfileId != senderProfileId && match.MatchedProfileId != senderProfileId))
            {
                throw new UnauthorizedAccessException("You don't have access to this conversation");
            }

            // Verify the match is accepted
            if (!match.IsAccepted)
            {
                throw new InvalidOperationException("Cannot send messages to an unaccepted match");
            }

            var message = new Message
            {
                MatchId = matchId,
                SenderProfileId = senderProfileId,
                Content = content.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var savedMessage = await _messageRepository.CreateMessageAsync(message);

            // Broadcast the message to all clients in the match group
            await _hubContext.Clients.Group($"match_{matchId}").SendAsync("ReceiveMessage", savedMessage);

            return savedMessage;
        }

        public async Task<Message> GetMessageByIdAsync(Guid messageId)
        {
            return await _messageRepository.GetMessageByIdAsync(messageId);
        }

        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid profileId)
        {
            return await _messageRepository.GetUnreadMessagesAsync(profileId);
        }

        public async Task MarkMessageAsReadAsync(Guid messageId, Guid profileId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                throw new ArgumentException("Message not found");
            }

            // Verify the user is the recipient of this message
            var match = await _matchService.GetMatchByIdAsync(message.MatchId);
            if (match == null || (match.ProfileId != profileId && match.MatchedProfileId != profileId))
            {
                throw new UnauthorizedAccessException("You don't have access to this message");
            }

            if (message.SenderProfileId == profileId)
            {
                throw new InvalidOperationException("Cannot mark your own message as read");
            }

            await _messageRepository.MarkMessageAsReadAsync(messageId);
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid profileId)
        {
            return await _messageRepository.GetUnreadMessageCountAsync(profileId);
        }
    }
} 