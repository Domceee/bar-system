namespace backend.Models;

public enum RouteStatus { Draft, Active, Finished, Cancelled }

public class Route
{
    public int Id { get; set; }
    public RouteStatus Status { get; set; } = RouteStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BarInRoute> Bars { get; set; } = [];
}
