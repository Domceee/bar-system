namespace backend.Models;

public class Reservation
{
    public int Id { get; set; }
    public int BarId { get; set; }
    public int GuestCount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public Bar Bar { get; set; } = null!;
    public List<Table> Tables { get; set; } = [];

    public void UpdateStatus(string status) => Status = status;
    public string CheckState() => Status;
    public void ChangeState() => Status = "cancelled";
    public void ChangeToEnded() => Status = "ended";

    public static Reservation Create(int barId, int guestCount, DateTime date, string status) =>
        new() { BarId = barId, GuestCount = guestCount, Date = date, Status = status };
}
