using backend.DTOs;
using backend.Models;

namespace backend.Services.Interfaces;

public interface ICocktailRecipeService
{
    Task<IEnumerable<CocktailRecipeDto>> GetAllAsync();
    Task<CocktailRecipeDto?> GetByIdAsync(int id);
    Task<CocktailRecipeDto> CreateAsync(CreateCocktailRecipeDto dto);
    Task<CocktailRecipeDto?> UpdateAsync(int id, UpdateCocktailRecipeDto dto);
    Task<bool> DeleteAsync(int id);

    Task<PublishRecipeResultDto> CheckPublishAsync(int id);
    Task<CocktailRecipeDto?> ConfirmPublishAsync(int id, PublishConfirmDto dto);

    Task<AbvPreviewResponseDto> PreviewAsync(AbvPreviewRequestDto dto);
    Task<OptimizeRecipeResponseDto> OptimizeAsync(int id, OptimizeRecipeRequestDto dto);
}
