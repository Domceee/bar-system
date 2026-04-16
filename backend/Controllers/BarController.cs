using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarController(IBarService barService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBars() =>
        Ok(await barService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBar(int id)
    {
        var bar = await barService.GetByIdAsync(id);
        return bar is null ? NotFound() : Ok(bar);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBar([FromBody] CreateBarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");

        var bar = await barService.CreateAsync(dto);
        return Ok(bar);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBar(int id, [FromBody] UpdateBarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");

        var bar = await barService.UpdateAsync(id, dto);
        return bar is null ? NotFound() : Ok(bar);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBar(int id)
    {
        var deleted = await barService.DeleteAsync(id);
        return deleted ? Ok() : NotFound();
    }
}
