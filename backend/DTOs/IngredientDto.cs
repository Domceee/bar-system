using backend.Models;

namespace backend.DTOs;

public record IngredientDto(int Id, string Name, IngredientCategory Category, double AbvPercentage, decimal Price);
public record CreateIngredientDto(string Name, IngredientCategory Category, double AbvPercentage, decimal Price);
public record UpdateIngredientDto(string Name, IngredientCategory Category, double AbvPercentage, decimal Price);
