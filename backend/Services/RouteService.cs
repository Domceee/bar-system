using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class RouteService(AppDbContext db) : IRouteService
{
    public async Task<RouteDto?> generateRoute(int routeId)
    {
        var route = await db.Routes
            .Include(r => r.Bars).ThenInclude(b => b.Bar)
            .FirstOrDefaultAsync(r => r.Id == routeId && r.Status == RouteStatus.Draft);
        if (route is null) return null;

        var optimized = optimizeRoute(route.Bars.ToList());
        for (var i = 0; i < optimized.Count; i++)
        {
            optimized[i].Order = i + 1;
            optimized[i].IsLast = i == optimized.Count - 1;
        }

        route.Status = RouteStatus.Draft;
        await db.SaveChangesAsync();
        return toDto(route, optimized);
    }

    public async Task<RouteDto?> startRoute(int routeId)
    {
        var route = await db.Routes
            .Include(r => r.Bars).ThenInclude(b => b.Bar)
            .FirstOrDefaultAsync(r => r.Id == routeId && r.Status == RouteStatus.Draft);
        if (route is null) return null;

        route.Status = RouteStatus.Active;
        await db.SaveChangesAsync();
        return toDto(route, route.Bars.OrderBy(b => b.Order).ToList());
    }

    public async Task<RouteDto?> cancelRoute(int routeId)
    {
        var route = await db.Routes.FindAsync(routeId);
        if (route is null) return null;

        route.Status = RouteStatus.Cancelled;
        await db.SaveChangesAsync();

        var bars = await db.BarsInRoute.Include(b => b.Bar)
            .Where(b => b.RouteId == routeId).OrderBy(b => b.Order).ToListAsync();
        return toDto(route, bars);
    }

    public async Task<BarInRouteDto?> getBarInRoute(int routeId)
    {
        var current = await db.BarsInRoute
            .Include(b => b.Bar)
            .Where(b => b.RouteId == routeId && !b.IsCompleted)
            .OrderBy(b => b.Order)
            .FirstOrDefaultAsync();
        return current is null ? null : toBarDto(current);
    }

    public async Task<RouteDto?> abortRoute(int routeId)
    {
        var current = await db.BarsInRoute
            .Include(b => b.Bar)
            .Where(b => b.RouteId == routeId && !b.IsCompleted)
            .OrderBy(b => b.Order)
            .FirstOrDefaultAsync();
        if (current is null) return null;

        current.IsLast = true;
        await db.SaveChangesAsync();

        var route = await db.Routes.FindAsync(routeId);
        var bars = await db.BarsInRoute.Include(b => b.Bar)
            .Where(b => b.RouteId == routeId).OrderBy(b => b.Order).ToListAsync();
        return toDto(route!, bars);
    }

    public async Task<RouteDto?> updateRoute(int routeId)
    {
        var current = await db.BarsInRoute
            .Include(b => b.Bar)
            .Where(b => b.RouteId == routeId && !b.IsCompleted)
            .OrderBy(b => b.Order)
            .FirstOrDefaultAsync();
        if (current is null) return null;

        current.IsCompleted = true;
        var route = await db.Routes.Include(r => r.Bars).ThenInclude(b => b.Bar)
            .FirstOrDefaultAsync(r => r.Id == routeId);
        if (route is null) return null;

        if (current.IsLast)
            route.Status = RouteStatus.Finished;

        await db.SaveChangesAsync();
        return toDto(route, route.Bars.OrderBy(b => b.Order).ToList());
    }

    private static List<BarInRoute> optimizeRoute(List<BarInRoute> bars)
    {
        if (bars.Count <= 1) return bars;

        var result = new List<BarInRoute>();
        var remaining = bars.ToList();
        var current = remaining[0];
        remaining.RemoveAt(0);
        result.Add(current);

        while (remaining.Count > 0)
        {
            var nearest = remaining
                .OrderBy(b => distance(current.Bar.XCoord, current.Bar.YCoord, b.Bar.XCoord, b.Bar.YCoord))
                .First();
            result.Add(nearest);
            remaining.Remove(nearest);
            current = nearest;
        }

        return result;
    }

    private static double distance(double x1, double y1, double x2, double y2) =>
        Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

    private static RouteDto toDto(Models.Route route, List<BarInRoute> bars) =>
        new(route.Id, route.Status.ToString(), bars.Select(toBarDto).ToList());

    private static BarInRouteDto toBarDto(BarInRoute b) =>
        new(b.Id, b.BarId, b.Bar.Name, b.Bar.Address, b.Order, b.IsLast, b.IsCompleted);
}
