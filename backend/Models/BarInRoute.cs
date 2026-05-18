namespace backend.Models;

public class BarInRoute
{
    public int Id { get; set; }
    public int RouteId { get; set; }
    public int BarId { get; set; }
    public int Order { get; set; }
    public bool IsLast { get; set; }
    public bool IsCompleted { get; set; } = false;

    public Route Route { get; set; } = null!;
    public Bar Bar { get; set; } = null!;
}
