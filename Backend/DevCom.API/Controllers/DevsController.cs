using DevCom.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevCom.API.Controllers;

[ApiController]
[Route("api/devs")]
public class DevsController(DevProfileService devProfileService) : ControllerBase
{
    /// <summary>
    /// Perfil público de um dev pelo e-mail (URL-encoded).
    /// Ex: GET /api/devs/joao%40email.com
    /// </summary>
    [HttpGet("{email}")]
    public async Task<IActionResult> GetProfile(string email, CancellationToken ct)
    {
        var decoded = Uri.UnescapeDataString(email);
        var result  = await devProfileService.GetByEmailAsync(decoded, ct);
        return Ok(result);
    }
}
