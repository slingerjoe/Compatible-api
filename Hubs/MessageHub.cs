using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Domain.DTOs;

namespace CompatibleAPI.Hubs
{
    /// <summary>
    /// SignalR hub for real-time messaging
    /// </summary>
    public class MessageHub : Hub
    {
        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a match group to receive messages for that specific match
        /// </summary>
        /// <param name="matchId">The ID of the match to join</param>
        public async Task JoinMatchGroup(string matchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"match_{matchId}");
        }

        /// <summary>
        /// Leave a match group
        /// </summary>
        /// <param name="matchId">The ID of the match to leave</param>
        public async Task LeaveMatchGroup(string matchId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"match_{matchId}");
        }

        /// <summary>
        /// Send a message to all clients in a specific match group
        /// This method is called by the server, not directly by clients
        /// </summary>
        /// <param name="matchId">The ID of the match</param>
        /// <param name="message">The message to send</param>
        public async Task SendMessageToMatch(string matchId, Message message)
        {
            var messageDto = new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SenderProfileId = message.SenderProfileId,
                MatchId = message.MatchId,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                ReadAt = message.ReadAt
            };
            
            await Clients.Group($"match_{matchId}").SendAsync("ReceiveMessage", messageDto);
        }

        /// <summary>
        /// Send a typing indicator to other users in the match
        /// </summary>
        /// <param name="matchId">The ID of the match</param>
        /// <param name="profileId">The ID of the profile that is typing</param>
        /// <param name="isTyping">Whether the user is typing or stopped typing</param>
        public async Task SendTypingIndicator(string matchId, string profileId, bool isTyping)
        {
            await Clients.Group($"match_{matchId}").SendAsync("UserTyping", profileId, isTyping);
        }
    }
} 