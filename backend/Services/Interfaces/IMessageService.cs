using backend.DTOs;

namespace backend.Services.Interfaces;

public interface IMessageService
{
    Task<List<MessageDto>> fetchAsync(int userId, int friendId);
    Task<MessageDto?> sendAsync(int senderId, int receiverId, string content);
}
