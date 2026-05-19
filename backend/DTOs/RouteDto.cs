namespace backend.DTOs;

public record BarInRouteDto(int Id, int BarId, string BarName, string Address, int Order, bool IsLast, bool IsCompleted);
public record RouteDto(int Id, string Status, List<BarInRouteDto> Bars);
public record RequestFittingBarsDto(List<int> UserIds);
public record SendInviteDto(int SenderId, int FriendUserId);
