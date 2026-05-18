using backend.DTOs;

namespace backend.Services.Interfaces;

public interface IFriendService
{
    Task<List<FriendDto>> fetchFriendsAsync(int userId);
    Task<FriendDto?> submitAsync(int requesterId, string targetUsername);
    Task<FriendDto?> acceptAsync(int requestId, int userId);
    Task<bool> removeFriendAsync(int friendId);
    Task<bool> sendInviteAsync(int senderId, int friendUserId);
}
