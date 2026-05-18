namespace backend.DTOs;

public record ReservationFormDto(int? BarId, int GuestCount, DateTime Date, double UserLat, double UserLon, bool UseNearestBar);
public record ConfirmReservationDto(int BarId, List<int> TableIds, int GuestCount, DateTime Date);
public record ReservationProposalDto(string Status, BarDto? Bar, List<int>? TableIds, string? WeatherForecast, string? ErrorMessage);
public record ReservationListItemDto(int Id, int BarId, string BarName, int GuestCount, DateTime Date, string Status, List<int> TableIds);
public record ReservationResponseDto(string Status, string? Message, List<ReservationListItemDto>? Reservations);
