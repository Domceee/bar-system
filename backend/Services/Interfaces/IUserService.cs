using backend.DTOs;
using backend.Models;

namespace backend.Services.Interfaces;

public interface IUserService
{
    Task<User> createUser(string username);
    TasteQuestionDto? nextQuestion(int index);
    Task<TasteProfileResponse?> submit(int userId, SubmitTasteProfileRequest request);
    Task<bool> openSurveyForm(int userId);
    Task<TasteProfileResponse?> fetchTasteProfile(int userId);
    Task<bool> delete(int userId);
    Task<TasteProfile?> getUserTasteProfile(int userId);
}
