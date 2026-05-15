using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlackjackController(IBlackjackService blackjackService) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile() =>
        Ok(await blackjackService.GetOrCreateProfileAsync());

    [HttpPost("chips/add")]
    public async Task<IActionResult> AddChips([FromBody] ChipsDto dto)
    {
        if (dto.Amount <= 0) return BadRequest("Amount must be positive.");
        return Ok(await blackjackService.AddChipsAsync(dto.Amount));
    }

    [HttpPost("chips/discard")]
    public async Task<IActionResult> DiscardChips([FromBody] ChipsDto dto)
    {
        if (dto.Amount <= 0) return BadRequest("Amount must be positive.");
        var profile = await blackjackService.DiscardChipsAsync(dto.Amount);
        return profile is null ? NotFound() : Ok(profile);
    }
}
