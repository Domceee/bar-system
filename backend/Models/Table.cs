namespace backend.Models;

public class Table
{
    public int Id { get; set; }
    public int BarId { get; set; }
    public int SeatCount { get; set; }
    public string Status { get; set; } = "available";
    public bool IsOutside { get; set; }
    public Bar Bar { get; set; } = null!;
    public List<Reservation> Reservations { get; set; } = [];

    public void UpdateStatus(string status) => Status = status;
    public void FreeTable() => Status = "available";

    public static List<Table> SelectFreeTables(List<Table> tables) =>
        tables.Where(t => t.Status == "available").ToList();
}
