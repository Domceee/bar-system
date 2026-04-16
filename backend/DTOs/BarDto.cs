namespace backend.DTOs;

public record CreateBarDto(string Name, double XCoord, double YCoord);
public record UpdateBarDto(string Name, double XCoord, double YCoord);
