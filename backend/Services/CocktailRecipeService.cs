using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CocktailRecipeService(AppDbContext db) : ICocktailRecipeService
{
    private const double DuplicateSimilarityThreshold = 0.8;

    public async Task<IEnumerable<CocktailRecipeDto>> GetAllAsync()
    {
        var recipes = await db.CocktailRecipes
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return recipes.Select(ToDto);
    }

    public async Task<CocktailRecipeDto?> GetByIdAsync(int id)
    {
        var recipe = await LoadWithIngredients(id);
        return recipe is null ? null : ToDto(recipe);
    }

    public async Task<CocktailRecipeDto> CreateAsync(CreateCocktailRecipeDto dto)
    {
        Validate(dto.Name, dto.Servings, dto.Ingredients);
        await EnsureIngredientsExist(dto.Ingredients);

        var recipe = new CocktailRecipe
        {
            Name = dto.Name.Trim(),
            Description = dto.Description ?? string.Empty,
            Servings = dto.Servings,
            AuthorId = dto.AuthorId,
            Status = RecipeStatus.Draft,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = dto.Ingredients
                .Select(i => new RecipeIngredient { IngredientId = i.IngredientId, Quantity = i.Quantity, Unit = i.Unit })
                .ToList()
        };

        recipe.ABV = await ComputeAbv(recipe.RecipeIngredients);

        db.CocktailRecipes.Add(recipe);
        await db.SaveChangesAsync();

        return (await GetByIdAsync(recipe.Id))!;
    }

    public async Task<CocktailRecipeDto?> UpdateAsync(int id, UpdateCocktailRecipeDto dto)
    {
        Validate(dto.Name, dto.Servings, dto.Ingredients);
        await EnsureIngredientsExist(dto.Ingredients);

        var recipe = await LoadWithIngredients(id);
        if (recipe is null) return null;

        recipe.Name = dto.Name.Trim();
        recipe.Description = dto.Description ?? string.Empty;
        recipe.Servings = dto.Servings;

        db.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
        recipe.RecipeIngredients = dto.Ingredients
            .Select(i => new RecipeIngredient { RecipeId = recipe.Id, IngredientId = i.IngredientId, Quantity = i.Quantity, Unit = i.Unit })
            .ToList();

        recipe.ABV = await ComputeAbv(recipe.RecipeIngredients);

        await db.SaveChangesAsync();
        return (await GetByIdAsync(recipe.Id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var recipe = await db.CocktailRecipes.FindAsync(id);
        if (recipe is null) return false;
        db.CocktailRecipes.Remove(recipe);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<PublishRecipeResultDto> CheckPublishAsync(int id)
    {
        var recipe = await LoadWithIngredients(id);
        if (recipe is null) return new PublishRecipeResultDto("notFound", null, null, null);

        var duplicate = await FindNearDuplicate(recipe);
        if (duplicate is not null)
        {
            var sim = Similarity(recipe, duplicate);
            return new PublishRecipeResultDto("duplicate", duplicate.Id, duplicate.Name, sim);
        }

        recipe.Status = RecipeStatus.Published;
        recipe.IsPublished = true;
        await db.SaveChangesAsync();
        return new PublishRecipeResultDto("published", null, null, null);
    }

    public async Task<CocktailRecipeDto?> ConfirmPublishAsync(int id, PublishConfirmDto dto)
    {
        var recipe = await LoadWithIngredients(id);
        if (recipe is null) return null;

        if (string.Equals(dto.Mode, "remix", StringComparison.OrdinalIgnoreCase))
        {
            if (!recipe.Name.Contains("(Remix)", StringComparison.OrdinalIgnoreCase))
                recipe.Name = $"{recipe.Name} (Remix)";
        }

        recipe.Status = RecipeStatus.Published;
        recipe.IsPublished = true;
        await db.SaveChangesAsync();
        return ToDto(recipe);
    }

    public async Task<AbvPreviewResponseDto> PreviewAsync(AbvPreviewRequestDto dto)
    {
        var temp = dto.Ingredients
            .Select(i => new RecipeIngredient { IngredientId = i.IngredientId, Quantity = i.Quantity, Unit = i.Unit })
            .ToList();
        var abv = await ComputeAbv(temp);
        var cost = await ComputeCost(temp);
        return new AbvPreviewResponseDto(abv, cost);
    }

    public async Task<OptimizeRecipeResponseDto> OptimizeAsync(int id, OptimizeRecipeRequestDto dto)
    {
        var recipe = await LoadWithIngredients(id);
        if (recipe is null)
            return new OptimizeRecipeResponseDto([], 0, 0, "Recipe not found.");

        var working = recipe.RecipeIngredients
            .Select(ri => new RecipeIngredientInputDto(ri.IngredientId, ri.Quantity, ri.Unit))
            .ToList();
        var notes = "";

        switch (dto.Mode?.ToLowerInvariant())
        {
            case "stronger":
                working = working
                    .Select(i => recipe.RecipeIngredients.First(r => r.IngredientId == i.IngredientId).Ingredient.AbvPercentage > 0
                        ? i with { Quantity = i.Quantity * 1.25 }
                        : i)
                    .ToList();
                notes = "Boosted alcoholic ingredients by 25%.";
                break;

            case "weaker":
                working = working
                    .Select(i => recipe.RecipeIngredients.First(r => r.IngredientId == i.IngredientId).Ingredient.AbvPercentage > 0
                        ? i with { Quantity = i.Quantity * 0.75 }
                        : i)
                    .ToList();
                notes = "Reduced alcoholic ingredients by 25%.";
                break;

            case "cheaper":
                working = await SubstituteCheaperAsync(recipe);
                notes = "Substituted ingredients in the same category for cheaper alternatives where possible.";
                break;

            case "changetype":
                if (dto.PreferCategory.HasValue)
                {
                    working = await SubstituteByCategoryAsync(recipe, dto.PreferCategory.Value);
                    notes = $"Steered base spirit toward {dto.PreferCategory.Value}.";
                }
                else
                {
                    notes = "No target category supplied.";
                }
                break;

            default:
                notes = "No optimization applied (unknown mode).";
                break;
        }

        var preview = working
            .Select(i => new RecipeIngredient { IngredientId = i.IngredientId, Quantity = i.Quantity, Unit = i.Unit })
            .ToList();
        var abv = await ComputeAbv(preview);
        var cost = await ComputeCost(preview);
        return new OptimizeRecipeResponseDto(working, abv, cost, notes);
    }

    // ---------- helpers ----------

    private async Task<CocktailRecipe?> LoadWithIngredients(int id) =>
        await db.CocktailRecipes
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

    private static void Validate(string name, int servings, List<RecipeIngredientInputDto> ingredients)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Recipe name is required.");
        if (servings <= 0)
            throw new ArgumentException("Servings must be greater than zero.");
        if (ingredients is null || ingredients.Count == 0)
            throw new ArgumentException("Recipe must have at least one ingredient.");
        if (ingredients.Any(i => i.Quantity <= 0))
            throw new ArgumentException("All ingredient quantities must be greater than zero.");
        var duplicateIds = ingredients.GroupBy(i => i.IngredientId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicateIds.Count > 0)
            throw new ArgumentException("Same ingredient listed multiple times.");
    }

    private async Task EnsureIngredientsExist(List<RecipeIngredientInputDto> ingredients)
    {
        var ids = ingredients.Select(i => i.IngredientId).Distinct().ToList();
        var existing = await db.Ingredients.Where(i => ids.Contains(i.Id)).Select(i => i.Id).ToListAsync();
        var missing = ids.Except(existing).ToList();
        if (missing.Count > 0)
            throw new ArgumentException($"Unknown ingredient ids: {string.Join(", ", missing)}");
    }

    private async Task<double> ComputeAbv(IEnumerable<RecipeIngredient> items)
    {
        var ids = items.Select(i => i.IngredientId).Distinct().ToList();
        var ingMap = await db.Ingredients.Where(i => ids.Contains(i.Id)).ToDictionaryAsync(i => i.Id);

        double totalMl = 0, alcoholMl = 0;
        foreach (var it in items)
        {
            if (!ingMap.TryGetValue(it.IngredientId, out var ing)) continue;
            var ml = ToMl(it.Quantity, it.Unit);
            totalMl += ml;
            alcoholMl += ml * (ing.AbvPercentage / 100.0);
        }
        return totalMl <= 0 ? 0 : Math.Round(alcoholMl / totalMl * 100.0, 2);
    }

    private async Task<decimal> ComputeCost(IEnumerable<RecipeIngredient> items)
    {
        var ids = items.Select(i => i.IngredientId).Distinct().ToList();
        var ingMap = await db.Ingredients.Where(i => ids.Contains(i.Id)).ToDictionaryAsync(i => i.Id);

        decimal cost = 0;
        foreach (var it in items)
        {
            if (!ingMap.TryGetValue(it.IngredientId, out var ing)) continue;
            var ml = ToMl(it.Quantity, it.Unit);
            // ingredient price is per typical retail unit; assume per 750ml for liquid, per piece otherwise
            var perUnitMl = ing.AbvPercentage > 0 ? 750.0 : 1000.0;
            cost += ing.Price * (decimal)(ml / perUnitMl);
        }
        return Math.Round(cost, 2);
    }

    private static double ToMl(double qty, IngredientUnit unit) => unit switch
    {
        IngredientUnit.Ml => qty,
        IngredientUnit.Cl => qty * 10,
        IngredientUnit.Oz => qty * 29.5735,
        IngredientUnit.Dash => qty * 0.92,
        IngredientUnit.Tsp => qty * 4.92892,
        IngredientUnit.Tbsp => qty * 14.7868,
        IngredientUnit.Piece => qty * 30,
        _ => qty
    };

    private async Task<CocktailRecipe?> FindNearDuplicate(CocktailRecipe candidate)
    {
        var published = await db.CocktailRecipes
            .Include(r => r.RecipeIngredients)
            .Where(r => r.IsPublished && r.Id != candidate.Id)
            .ToListAsync();

        foreach (var existing in published)
        {
            if (string.Equals(existing.Name.Trim(), candidate.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                return existing;
            if (Similarity(candidate, existing) >= DuplicateSimilarityThreshold)
                return existing;
        }
        return null;
    }

    private static double Similarity(CocktailRecipe a, CocktailRecipe b)
    {
        var setA = a.RecipeIngredients.Select(ri => ri.IngredientId).ToHashSet();
        var setB = b.RecipeIngredients.Select(ri => ri.IngredientId).ToHashSet();
        if (setA.Count == 0 || setB.Count == 0) return 0;
        var intersect = setA.Intersect(setB).Count();
        var union = setA.Union(setB).Count();
        return union == 0 ? 0 : (double)intersect / union;
    }

    private async Task<List<RecipeIngredientInputDto>> SubstituteCheaperAsync(CocktailRecipe recipe)
    {
        var allIngredients = await db.Ingredients.ToListAsync();
        var result = new List<RecipeIngredientInputDto>();
        foreach (var ri in recipe.RecipeIngredients)
        {
            var cheaper = allIngredients
                .Where(i => i.Category == ri.Ingredient.Category && i.Price < ri.Ingredient.Price)
                .OrderBy(i => i.Price)
                .FirstOrDefault();
            result.Add(new RecipeIngredientInputDto(cheaper?.Id ?? ri.IngredientId, ri.Quantity, ri.Unit));
        }
        return result;
    }

    private async Task<List<RecipeIngredientInputDto>> SubstituteByCategoryAsync(CocktailRecipe recipe, IngredientCategory target)
    {
        var swapTarget = await db.Ingredients
            .Where(i => i.Category == target)
            .OrderByDescending(i => i.AbvPercentage)
            .FirstOrDefaultAsync();
        if (swapTarget is null)
            return recipe.RecipeIngredients
                .Select(ri => new RecipeIngredientInputDto(ri.IngredientId, ri.Quantity, ri.Unit))
                .ToList();

        var result = new List<RecipeIngredientInputDto>();
        var swapped = false;
        foreach (var ri in recipe.RecipeIngredients)
        {
            if (!swapped && ri.Ingredient.AbvPercentage > 0 && ri.Ingredient.Category != target)
            {
                result.Add(new RecipeIngredientInputDto(swapTarget.Id, ri.Quantity, ri.Unit));
                swapped = true;
            }
            else
            {
                result.Add(new RecipeIngredientInputDto(ri.IngredientId, ri.Quantity, ri.Unit));
            }
        }
        return result;
    }

    private static CocktailRecipeDto ToDto(CocktailRecipe r) =>
        new(
            r.Id,
            r.Name,
            r.Description,
            r.Status,
            r.IsPublished,
            r.ABV,
            r.Servings,
            r.CreatedAt,
            r.AuthorId,
            r.RecipeIngredients.Select(ri => new RecipeIngredientDto(
                ri.Id,
                ri.IngredientId,
                ri.Ingredient?.Name ?? string.Empty,
                ri.Ingredient?.Category ?? IngredientCategory.Other,
                ri.Ingredient?.AbvPercentage ?? 0,
                ri.Ingredient?.Price ?? 0,
                ri.Quantity,
                ri.Unit
            )).ToList()
        );
}
