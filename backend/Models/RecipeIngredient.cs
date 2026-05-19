namespace backend.Models;

public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public CocktailRecipe Recipe { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public double Quantity { get; set; }
    public IngredientUnit Unit { get; set; }
}
