using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FriendController(IFriendService friendService) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> fetchFriends(int userId) =>
        Ok(await friendService.fetchFriendsAsync(userId));

    [HttpPost]
    public async Task<IActionResult> submit([FromBody] SendFriendRequestDto dto)
    {
        var result = await friendService.submitAsync(dto.RequesterId, dto.TargetUsername);
        return result is null ? BadRequest("User not found or request already exists.") : Ok(result);
    }

    [HttpPost("accept")]
    public async Task<IActionResult> accept([FromBody] AcceptFriendRequestDto dto)
    {
        var result = await friendService.acceptAsync(dto.RequestId, dto.UserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{friendId:int}")]
    public async Task<IActionResult> removeFriend(int friendId)
    {
        var success = await friendService.removeFriendAsync(friendId);
        return success ? Ok() : NotFound();
    }

    [HttpPost("invite")]
    public async Task<IActionResult> sendInvite([FromBody] SendInviteDto dto)
    {
        var success = await friendService.sendInviteAsync(dto.SenderId, dto.FriendUserId);
        return success ? Ok() : NotFound("User not found.");
    }
}
