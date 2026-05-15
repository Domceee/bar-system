using backend.Data;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

public class BlackjackService(AppDbContext db) : IBlackjackService
{
    public async Task<BlackjackProfile> GetOrCreateProfileAsync()
    {
        var profile = await db.BlackjackProfiles.FindAsync(1);
        if (profile is null)
        {
            profile = new BlackjackProfile { Id = 1, Chips = 1000 };
            db.BlackjackProfiles.Add(profile);
            await db.SaveChangesAsync();
        }
        return profile;
    }

    public async Task<BlackjackProfile> AddChipsAsync(int amount)
    {
        var profile = await GetOrCreateProfileAsync();
        profile.Chips += amount;
        await db.SaveChangesAsync();
        return profile;
    }

    public async Task<BlackjackProfile?> DiscardChipsAsync(int amount)
    {
        var profile = await db.BlackjackProfiles.FindAsync(1);
        if (profile is null) return null;
        profile.Chips = Math.Max(0, profile.Chips - amount);
        await db.SaveChangesAsync();
        return profile;
    }
}
