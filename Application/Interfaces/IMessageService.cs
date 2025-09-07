using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessagesByMatchIdAsync(Guid matchId, Guid profileId);
        Task<Message> SendMessageAsync(Guid matchId, Guid senderProfileId, string content);
        Task<Message> GetMessageByIdAsync(Guid messageId);
        Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid profileId);
        Task MarkMessageAsReadAsync(Guid messageId, Guid profileId);
        Task<int> GetUnreadMessageCountAsync(Guid profileId);
    }
} 