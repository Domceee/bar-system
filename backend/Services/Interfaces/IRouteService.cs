using backend.DTOs;

namespace backend.Services.Interfaces;

public interface IRouteService
{
    Task<RouteDto?> generateRoute(int routeId);
    Task<RouteDto?> startRoute(int routeId);
    Task<RouteDto?> cancelRoute(int routeId);
    Task<BarInRouteDto?> getBarInRoute(int routeId);
    Task<RouteDto?> abortRoute(int routeId);
    Task<RouteDto?> updateRoute(int routeId);
}
