using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController(IRouteService routeService) : ControllerBase
{
    [HttpPost("generate/{routeId:int}")]
    public async Task<IActionResult> generateRoute(int routeId)
    {
        var result = await routeService.generateRoute(routeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("start/{routeId:int}")]
    public async Task<IActionResult> startRoute(int routeId)
    {
        var result = await routeService.startRoute(routeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("cancel/{routeId:int}")]
    public async Task<IActionResult> cancelRoute(int routeId)
    {
        var result = await routeService.cancelRoute(routeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{routeId:int}/current")]
    public async Task<IActionResult> getBarInRoute(int routeId)
    {
        var result = await routeService.getBarInRoute(routeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("abort/{routeId:int}")]
    public async Task<IActionResult> abortRoute(int routeId)
    {
        var result = await routeService.abortRoute(routeId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("update/{routeId:int}")]
    public async Task<IActionResult> updateRoute(int routeId)
    {
        var result = await routeService.updateRoute(routeId);
        return result is null ? NotFound() : Ok(result);
    }
}
