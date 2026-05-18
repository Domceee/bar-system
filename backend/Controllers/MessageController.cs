using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(IMessageService messageService) : ControllerBase
{
    [HttpGet("{userId:int}/{friendId:int}")]
    public async Task<IActionResult> fetch(int userId, int friendId) =>
        Ok(await messageService.fetchAsync(userId, friendId));

    [HttpPost]
    public async Task<IActionResult> send([FromBody] SendMessageDto dto)
    {
        var result = await messageService.sendAsync(dto.SenderId, dto.ReceiverId, dto.Content);
        return result is null ? BadRequest("Invalid sender or empty message.") : Ok(result);
    }
}
