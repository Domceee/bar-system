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
        new("sweetness", "How much do you like sweet flavors?",
            ["Don't like it", "Neutral", "Love it"], 0),
        new("bitterness", "How much do you like bitter flavors?",
            ["Don't like it", "Neutral", "Love it"], 0),
        new("drink_type", "Which drink do you choose most often?",
            ["Beer", "Wine", "Cocktail", "Spirits"], 0),
        new("alcohol_strength", "What strength of drinks do you prefer?",
            ["Light", "Medium", "Strong"], 0),
        new("flavor_profile", "What's your favorite flavor profile?",
            ["Fruity", "Herbal", "Sweet", "Spicy"], 0)
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
}
