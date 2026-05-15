namespace backend.Models;

public class Bar
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double XCoord { get; set; }
    public double YCoord { get; set; }
    public double Rating { get; set; }
    public string Address { get; set; } = string.Empty;
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public BarDesign Design { get; set; }

    public List<Drink> Drinks { get; set; } = [];

    public Bar SelectBar() => this;
}
