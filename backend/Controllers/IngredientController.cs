using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController(IIngredientService ingredients) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search) =>
        Ok(await ingredients.GetAllAsync(search));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ing = await ingredients.GetByIdAsync(id);
        return ing is null ? NotFound() : Ok(ing);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIngredientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ing = await ingredients.CreateAsync(dto);
        return Ok(ing);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIngredientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ing = await ingredients.UpdateAsync(id, dto);
        return ing is null ? NotFound() : Ok(ing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await ingredients.DeleteAsync(id);
        return deleted ? Ok() : NotFound();
    }
}
