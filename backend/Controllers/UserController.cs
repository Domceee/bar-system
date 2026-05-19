using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> createUser([FromBody] string username) =>
        Ok(await userService.createUser(username));

    [HttpGet("next-question/{index:int}")]
    public ActionResult<TasteQuestionDto> nextQuestion(int index)
    {
        var question = userService.nextQuestion(index);
        return question is null ? NotFound() : Ok(question);
    }

    [HttpPost("{userId:int}/taste-profile")]
    public async Task<IActionResult> submit(int userId, [FromBody] SubmitTasteProfileRequest request)
    {
        var response = await userService.submit(userId, request);
        return response is null
            ? NotFound($"User {userId} not found")
            : CreatedAtAction(nameof(submit), new { userId }, response);
    }

    [HttpGet("{userId:int}/survey-form")]
    public async Task<IActionResult> openSurveyForm(int userId)
    {
        var surveyNeeded = await userService.openSurveyForm(userId);
        return Ok(new { surveyNeeded });
    }

    [HttpGet("{userId:int}/taste-profile")]
    public async Task<IActionResult> fetchTasteProfile(int userId)
    {
        var profile = await userService.fetchTasteProfile(userId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpDelete("{userId:int}/taste-profile")]
    public async Task<IActionResult> delete(int userId)
    {
        var deleted = await userService.delete(userId);
        return deleted ? Ok() : NotFound();
    }
}
