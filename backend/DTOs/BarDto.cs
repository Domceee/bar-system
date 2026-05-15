using backend.Models;

namespace backend.DTOs;

public record BarDto(int Id, string Name, double XCoord, double YCoord, double Rating, string Address, TimeOnly OpenTime, TimeOnly CloseTime, BarDesign Design, BarAtmosphere Atmosphere, BarSeating Seating);
public record CreateBarDto(string Name, double XCoord, double YCoord, double Rating, string Address, TimeOnly OpenTime, TimeOnly CloseTime, BarDesign Design, BarAtmosphere Atmosphere, BarSeating Seating);
public record UpdateBarDto(string Name, double XCoord, double YCoord, double Rating, string Address, TimeOnly OpenTime, TimeOnly CloseTime, BarDesign Design, BarAtmosphere Atmosphere, BarSeating Seating);
