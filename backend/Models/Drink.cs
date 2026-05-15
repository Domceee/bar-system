namespace backend.Models;

public class Drink
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DrinkType Type { get; set; }
    public decimal Price { get; set; }
    public DrinkFlavor Flavor { get; set; }

    public int BarId { get; set; }
    public Bar Bar { get; set; } = null!;
}
