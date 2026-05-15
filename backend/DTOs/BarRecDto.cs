namespace backend.DTOs;

public record BarsWithinDistanceRequest(int UserId, double Lat, double Lon, double DistanceMeters);

public record NearbyBar(string Name, double Lat, double Lon, string PlaceId, double? Rating);

public record DbBarDto(int Id, string Name, double XCoord, double YCoord, double Rating, string Design, int Priority);

public record BarsWithinDistanceResponse(
    string Status,
    List<NearbyBar> Bars,
    List<DbBarDto> DbBars);
