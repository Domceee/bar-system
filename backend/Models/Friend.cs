namespace backend.Models;

public enum FriendStatus { Pending, Accepted }

public class Friend
{
    public int Id { get; set; }
    public int RequesterId { get; set; }
    public int AddresseeId { get; set; }
    public FriendStatus Status { get; set; } = FriendStatus.Pending;

    public User Requester { get; set; } = null!;
    public User Addressee { get; set; } = null!;
}
