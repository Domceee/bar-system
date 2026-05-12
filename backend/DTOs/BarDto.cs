namespace backend.DTOs;

public record BarDto(int Id, string Name, double XCoord, double YCoord, double Rating, string Address, TimeOnly OpenTime, TimeOnly CloseTime);
public record CreateBarDto(string Name, double XCoord, double YCoord, double Rating, string Address, TimeOnly OpenTime, TimeOnly CloseTime);
public record UpdateBarDto(string Name, double XCoord, double YCoord, double Rating, string Address, TimeOnly OpenTime, TimeOnly CloseTime);
