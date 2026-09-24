using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevCom.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCom.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(NotificationService notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await notificationService.GetByUserAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await notificationService.GetUnreadCountAsync(userId, ct);
        return Ok(result);
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        await notificationService.MarkAllReadAsync(userId, ct);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? throw new UnauthorizedAccessException("Token invalido.");
        return int.Parse(value);
    }
}
