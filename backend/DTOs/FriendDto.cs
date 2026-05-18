namespace backend.DTOs;

public record FriendDto(int Id, int FriendUserId, string FriendUsername, string Status);
public record SendFriendRequestDto(int RequesterId, string TargetUsername);
public record AcceptFriendRequestDto(int RequestId, int UserId);
