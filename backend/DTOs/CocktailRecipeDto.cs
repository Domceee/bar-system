using backend.Models;

namespace backend.DTOs;

public record RecipeIngredientInputDto(int IngredientId, double Quantity, IngredientUnit Unit);
public record RecipeIngredientDto(int Id, int IngredientId, string IngredientName, IngredientCategory Category, double AbvPercentage, decimal Price, double Quantity, IngredientUnit Unit);

public record CocktailRecipeDto(
    int Id,
    string Name,
    string Description,
    RecipeStatus Status,
    bool IsPublished,
    double ABV,
    int Servings,
    DateTime CreatedAt,
    int? AuthorId,
    List<RecipeIngredientDto> Ingredients);

public record CreateCocktailRecipeDto(
    string Name,
    string Description,
    int Servings,
    int? AuthorId,
    List<RecipeIngredientInputDto> Ingredients);

public record UpdateCocktailRecipeDto(
    string Name,
    string Description,
    int Servings,
    List<RecipeIngredientInputDto> Ingredients);

public record PublishRecipeResultDto(string Status, int? DuplicateOfId, string? DuplicateOfName, double? Similarity);
public record PublishConfirmDto(string Mode); // "remix" | "original"

public record OptimizeRecipeRequestDto(string Mode, double? TargetAbv, IngredientCategory? PreferCategory);
public record OptimizeRecipeResponseDto(List<RecipeIngredientInputDto> Ingredients, double EstimatedAbv, decimal EstimatedCost, string Notes);

public record AbvPreviewRequestDto(int Servings, List<RecipeIngredientInputDto> Ingredients);
public record AbvPreviewResponseDto(double ABV, decimal Cost);
