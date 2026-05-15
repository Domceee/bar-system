namespace backend.Models;

public class TasteProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TasteAnswer> Answers { get; set; } = [];
}
