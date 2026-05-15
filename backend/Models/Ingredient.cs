namespace backend.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IngredientCategory Category { get; set; }
    public double AbvPercentage { get; set; }
    public decimal Price { get; set; }
}
