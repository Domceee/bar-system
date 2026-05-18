using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class FriendService(AppDbContext db) : IFriendService
{
    public async Task<List<FriendDto>> fetchFriendsAsync(int userId)
    {
        var friends = await db.Friends
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .Where(f => f.RequesterId == userId || f.AddresseeId == userId)
            .ToListAsync();

        return friends.Select(f =>
        {
            var iAmRequester = f.RequesterId == userId;
            var friendUser = iAmRequester ? f.Addressee : f.Requester;
            return new FriendDto(f.Id, friendUser.Id, friendUser.Username, f.Status.ToString());
        }).ToList();
    }

    public async Task<FriendDto?> submitAsync(int requesterId, string targetUsername)
    {
        var addressee = await db.Users.FirstOrDefaultAsync(u => u.Username == targetUsername);
        if (addressee is null || addressee.Id == requesterId) return null;

        var exists = await db.Friends.AnyAsync(f =>
            (f.RequesterId == requesterId && f.AddresseeId == addressee.Id) ||
            (f.RequesterId == addressee.Id && f.AddresseeId == requesterId));
        if (exists) return null;

        var invite = createInvite(requesterId, addressee.Id);
        db.Friends.Add(invite);
        await db.SaveChangesAsync();

        return new FriendDto(invite.Id, addressee.Id, addressee.Username, invite.Status.ToString());
    }

    public async Task<FriendDto?> acceptAsync(int requestId, int userId)
    {
        var friend = await db.Friends
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .FirstOrDefaultAsync(f => f.Id == requestId && (f.AddresseeId == userId || f.RequesterId == userId));
        if (friend is null) return null;

        friend.Status = FriendStatus.Accepted;
        await db.SaveChangesAsync();

        return new FriendDto(friend.Id, friend.Requester.Id, friend.Requester.Username, friend.Status.ToString());
    }

    public async Task<bool> removeFriendAsync(int friendId)
    {
        var friend = await db.Friends.FindAsync(friendId);
        if (friend is null) return false;

        db.Friends.Remove(friend);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> sendInviteAsync(int senderId, int friendUserId)
    {
        var sender = await db.Users.FindAsync(senderId);
        var target = await db.Users.FindAsync(friendUserId);
        return sender is not null && target is not null;
    }

    private static Friend createInvite(int requesterId, int addresseeId) =>
        new() { RequesterId = requesterId, AddresseeId = addresseeId, Status = FriendStatus.Accepted };
}
