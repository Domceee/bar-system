using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class IngredientService(AppDbContext db) : IIngredientService
{
    public async Task<IEnumerable<Ingredient>> GetAllAsync(string? search)
    {
        var query = db.Ingredients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(i => i.Name.ToLower().Contains(s));
        }
        return await query.OrderBy(i => i.Name).ToListAsync();
    }

    public async Task<Ingredient?> GetByIdAsync(int id) =>
        await db.Ingredients.FindAsync(id);

    public async Task<Ingredient> CreateAsync(CreateIngredientDto dto)
    {
        var ing = new Ingredient
        {
            Name = dto.Name,
            Category = dto.Category,
            AbvPercentage = dto.AbvPercentage,
            Price = dto.Price
        };
        db.Ingredients.Add(ing);
        await db.SaveChangesAsync();
        return ing;
    }

    public async Task<Ingredient?> UpdateAsync(int id, UpdateIngredientDto dto)
    {
        var ing = await db.Ingredients.FindAsync(id);
        if (ing is null) return null;
        ing.Name = dto.Name;
        ing.Category = dto.Category;
        ing.AbvPercentage = dto.AbvPercentage;
        ing.Price = dto.Price;
        await db.SaveChangesAsync();
        return ing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ing = await db.Ingredients.FindAsync(id);
        if (ing is null) return false;
        db.Ingredients.Remove(ing);
        await db.SaveChangesAsync();
        return true;
    }
}
