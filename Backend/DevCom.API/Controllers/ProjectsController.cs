using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevCom.Application.DTOs.Projects;
using DevCom.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCom.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(ProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProjectFilterRequest filter, CancellationToken ct)
    {
        var result = await projectService.GetPagedAsync(filter, ct);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await projectService.GetMyProjectsAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await projectService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await projectService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? throw new UnauthorizedAccessException("Token invalido.");
        return int.Parse(value);
    }
}
