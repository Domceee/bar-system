namespace backend.Models;

public class CocktailRecipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecipeStatus Status { get; set; }
    public bool IsPublished { get; set; }
    public double ABV { get; set; }
    public int Servings { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? AuthorId { get; set; }
    public User? Author { get; set; }

    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
