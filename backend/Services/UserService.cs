using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService(AppDbContext db) : IUserService
{
    private static readonly List<TasteQuestionDto> TasteQuestions =
    [
        new("budget", "What's your drink budget per drink?",
            ["$1-5", "$5-10", "$10-15", "$15-25", "$25+"], 0),
        new("flavor_balance", "Which flavor balance do you prefer?",
            ["Sweet", "Bitter", "Sour", "Mixed"], 0),
        new("drink_strength", "What strength of drinks do you prefer?",
            ["Light", "Medium", "Strong"], 0),
        new("drink_type", "What's your go-to drink?",
            ["Beer", "Cider", "Cocktails", "Shots", "Wine"], 0),
        new("flavor_profile", "What's your favorite flavor profile?",
            ["Fruity", "Herbal", "Smoky", "Spicy", "Sweet"], 0),
        new("bar_distance", "How far are you willing to travel for a bar?",
            ["Under 1 km", "1-5 km", "5-15 km", "Any distance"], 0),
        new("bar_rating", "What's the minimum bar rating you'd accept?",
            ["1 star", "2 stars", "3 stars", "4 stars", "5 stars"], 0),
        new("bar_design", "What kind of bar interior do you prefer?",
            ["Cozy", "Modern", "Vintage", "Industrial", "Luxurious"], 0)
    ];

    public async Task<User> createUser(string username)
    {
        var user = new User { Username = username };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public TasteQuestionDto? nextQuestion(int index)
    {
        if (index < 0 || index >= TasteQuestions.Count) return null;
        return TasteQuestions[index] with { TotalCount = TasteQuestions.Count };
    }

    public async Task<TasteProfileResponse?> submit(int userId, SubmitTasteProfileRequest request)
    {
        var userExists = await db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return null;

        var profile = new TasteProfile
        {
            UserId = userId,
            Answers = request.Answers
                .Select(a => new TasteAnswer
                {
                    QuestionKey = a.QuestionKey,
                    Answer = a.Answer
                })
                .ToList()
        };

        db.TasteProfiles.Add(profile);
        await db.SaveChangesAsync();

        return new TasteProfileResponse(
            profile.Id,
            profile.UserId,
            profile.CreatedAt,
            profile.Answers
                .Select(a => new TasteAnswerDto(a.QuestionKey, a.Answer))
                .ToList());
    }

    public async Task<bool> openSurveyForm(int userId) =>
        !await db.TasteProfiles.AnyAsync(p => p.UserId == userId);

    public async Task<TasteProfileResponse?> fetchTasteProfile(int userId)
    {
        var profile = await db.TasteProfiles
            .Include(p => p.Answers)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (profile is null) return null;

        return new TasteProfileResponse(
            profile.Id,
            profile.UserId,
            profile.CreatedAt,
            profile.Answers.Select(a => new TasteAnswerDto(a.QuestionKey, a.Answer)).ToList());
    }

    public async Task<bool> delete(int userId)
    {
        var profiles = await db.TasteProfiles.Where(p => p.UserId == userId).ToListAsync();
        if (profiles.Count == 0) return false;
        db.TasteProfiles.RemoveRange(profiles);
        await db.SaveChangesAsync();
        return true;
    }
}
