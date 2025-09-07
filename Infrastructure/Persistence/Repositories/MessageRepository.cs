using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Infrastructure.Persistence;

namespace CompatibleAPI.Infrastructure.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetMessagesByMatchIdAsync(Guid matchId)
        {
            return await _context.Messages
                .Include(m => m.SenderProfile)
                .Where(m => m.MatchId == matchId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            return await _context.Messages
                .Include(m => m.SenderProfile)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid profileId)
        {
            return await _context.Messages
                .Include(m => m.SenderProfile)
                .Include(m => m.Match)
                .Where(m => m.Match.ProfileId == profileId || m.Match.MatchedProfileId == profileId)
                .Where(m => m.SenderProfileId != profileId && !m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkMessageAsReadAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid profileId)
        {
            return await _context.Messages
                .Include(m => m.Match)
                .Where(m => m.Match.ProfileId == profileId || m.Match.MatchedProfileId == profileId)
                .Where(m => m.SenderProfileId != profileId && !m.IsRead)
                .CountAsync();
        }
    }
} 