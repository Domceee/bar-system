namespace backend.DTOs;

public record TableDto(int Id, int BarId, int SeatCount, string Status, bool IsOutside);
