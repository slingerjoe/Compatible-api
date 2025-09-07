using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesByMatchIdAsync(Guid matchId);
        Task<Message> CreateMessageAsync(Message message);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid profileId);
        Task MarkMessageAsReadAsync(Guid messageId);
        Task<int> GetUnreadMessageCountAsync(Guid profileId);
    }
} 