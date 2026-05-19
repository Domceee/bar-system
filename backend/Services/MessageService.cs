using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class MessageService(AppDbContext db) : IMessageService
{
    public async Task<List<MessageDto>> fetchAsync(int userId, int friendId)
    {
        var messages = await db.Messages
            .Include(m => m.Sender)
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == friendId) ||
                (m.SenderId == friendId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return messages.Select(m =>
            new MessageDto(m.Id, m.SenderId, m.Sender.Username, m.Content, m.SentAt)
        ).ToList();
    }

    public async Task<MessageDto?> sendAsync(int senderId, int receiverId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;

        var sender = await db.Users.FindAsync(senderId);
        if (sender is null) return null;

        var message = new Message { SenderId = senderId, ReceiverId = receiverId, Content = content };
        db.Messages.Add(message);
        await db.SaveChangesAsync();

        return new MessageDto(message.Id, message.SenderId, sender.Username, message.Content, message.SentAt);
    }
}
