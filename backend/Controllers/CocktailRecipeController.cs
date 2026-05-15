using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CocktailRecipeController(ICocktailRecipeService recipes) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await recipes.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var r = await recipes.GetByIdAsync(id);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCocktailRecipeDto dto)
    {
        try
        {
            var r = await recipes.CreateAsync(dto);
            return Ok(r);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCocktailRecipeDto dto)
    {
        try
        {
            var r = await recipes.UpdateAsync(id, dto);
            return r is null ? NotFound() : Ok(r);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await recipes.DeleteAsync(id);
        return deleted ? Ok() : NotFound();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(int id) =>
        Ok(await recipes.CheckPublishAsync(id));

    [HttpPost("{id}/publish/confirm")]
    public async Task<IActionResult> ConfirmPublish(int id, [FromBody] PublishConfirmDto dto)
    {
        var r = await recipes.ConfirmPublishAsync(id, dto);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpPost("preview")]
    public async Task<IActionResult> Preview([FromBody] AbvPreviewRequestDto dto) =>
        Ok(await recipes.PreviewAsync(dto));

    [HttpPost("{id}/optimize")]
    public async Task<IActionResult> Optimize(int id, [FromBody] OptimizeRecipeRequestDto dto) =>
        Ok(await recipes.OptimizeAsync(id, dto));
}
