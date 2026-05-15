using backend.Models;

namespace backend.Services.Interfaces;

public interface IBlackjackService
{
    Task<BlackjackProfile> GetOrCreateProfileAsync();
    Task<BlackjackProfile> AddChipsAsync(int amount);
    Task<BlackjackProfile?> DiscardChipsAsync(int amount);
}
