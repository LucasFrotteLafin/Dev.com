using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevCom.Application.DTOs.Proposals;
using DevCom.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCom.API.Controllers;

[ApiController]
public class ProposalsController(ProposalService proposalService) : ControllerBase
{
    [HttpGet("api/projects/{projectId:int}/proposals")]
    [Authorize]
    public async Task<IActionResult> GetByProject(int projectId, CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await proposalService.GetByProjectAsync(projectId, userId, ct);
        return Ok(result);
    }

    [HttpPost("api/projects/{projectId:int}/proposals")]
    [Authorize]
    public async Task<IActionResult> Create(
        int projectId, [FromBody] CreateProposalRequest request, CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await proposalService.CreateAsync(projectId, userId, request, ct);
        return Ok(result);
    }

    [HttpGet("api/proposals/my")]
    [Authorize]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        var result = await proposalService.GetMyProposalsAsync(userId, ct);
        return Ok(result);
    }

    [HttpPatch("api/proposals/{id:int}/accept")]
    [Authorize]
    public async Task<IActionResult> Accept(int id, CancellationToken ct)
    {
        int userId = GetCurrentUserId();
        await proposalService.AcceptAsync(id, userId, ct);
        return Ok(new { message = "Proposta aceita com sucesso." });
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? throw new UnauthorizedAccessException("Token invalido.");
        return int.Parse(value);
    }
}
