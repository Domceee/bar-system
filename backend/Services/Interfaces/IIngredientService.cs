using backend.DTOs;
using backend.Models;

namespace backend.Services.Interfaces;

public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllAsync(string? search);
    Task<Ingredient?> GetByIdAsync(int id);
    Task<Ingredient> CreateAsync(CreateIngredientDto dto);
    Task<Ingredient?> UpdateAsync(int id, UpdateIngredientDto dto);
    Task<bool> DeleteAsync(int id);
}
