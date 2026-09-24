using DevCom.Application.DTOs.Proposals;
using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Domain.Enums;

namespace DevCom.Application.Services;

public class ProposalService(
    IProposalRepository     proposalRepo,
    IProjectRepository      projectRepo,
    IUserRepository         userRepo,
    INotificationRepository notifRepo,
    ICacheService           cache)
{
    public async Task<List<ProposalResponse>> GetByProjectAsync(
        int projectId, int requesterId, CancellationToken ct = default)
    {
        var project   = await projectRepo.GetByIdWithDetailsAsync(projectId, ct)
                        ?? throw new KeyNotFoundException("Projeto nao encontrado.");
        var requester = await userRepo.GetByIdAsync(requesterId, ct)
                        ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        bool isOwner = project.OwnerId == requesterId;
        bool isDev   = requester.Role == UserRole.Dev;

        if (!isOwner && !isDev)
            throw new UnauthorizedAccessException("Acesso negado.");

        var proposals = await proposalRepo.GetByProjectAsync(projectId, ct);

        // Fix: Dev so ve a propria proposta; dono ve todas
        if (isDev && !isOwner)
            proposals = proposals.Where(p => p.DevId == requesterId).ToList();

        return proposals.Select(p => MapToResponse(p)).ToList();
    }

    public async Task<List<ProposalResponse>> GetMyProposalsAsync(int devId, CancellationToken ct = default)
    {
        var proposals = await proposalRepo.GetByDevAsync(devId, ct);
        return proposals.Select(p => MapToResponse(p, includeProjectTitle: true)).ToList();
    }

    public async Task<ProposalResponse> CreateAsync(
        int projectId, int devId, CreateProposalRequest request, CancellationToken ct = default)
    {
        var dev = await userRepo.GetByIdAsync(devId, ct)
                  ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        if (dev.Role != UserRole.Dev)
            throw new UnauthorizedAccessException("Apenas desenvolvedores podem enviar propostas.");

        var project = await projectRepo.GetByIdWithDetailsAsync(projectId, ct)
                      ?? throw new KeyNotFoundException("Projeto nao encontrado.");

        if (project.Status != ProjectStatus.Open)
            throw new InvalidOperationException("Este projeto nao esta aceitando propostas.");

        if (project.OwnerId == devId)
            throw new InvalidOperationException("Voce nao pode enviar proposta para o seu proprio projeto.");

        bool alreadySent = await proposalRepo.ExistsAsync(projectId, devId, ct);
        if (alreadySent)
            throw new InvalidOperationException("Voce ja enviou uma proposta para este projeto.");

        var proposal = new Proposal
        {
            ProjectId = projectId,
            DevId     = devId,
            Value     = request.Value.Trim(),
            Deadline  = request.Deadline.Trim(),
            Message   = request.Message.Trim()
        };

        await proposalRepo.AddAsync(proposal, ct);
        await proposalRepo.SaveChangesAsync(ct);

        await notifRepo.AddAsync(new Notification
        {
            UserId    = project.OwnerId,
            Message   = $"{dev.Name} enviou uma proposta para \"{project.Title}\".",
            ProjectId = projectId
        }, ct);
        await notifRepo.SaveChangesAsync(ct);

        await cache.RemoveAsync($"project_{projectId}", ct);

        proposal.Dev     = dev;
        proposal.Project = project;
        return MapToResponse(proposal);
    }

    public async Task AcceptAsync(int proposalId, int requesterId, CancellationToken ct = default)
    {
        var stub = await proposalRepo.GetByIdAsync(proposalId, ct)
                   ?? throw new KeyNotFoundException("Proposta nao encontrada.");

        var project = await projectRepo.GetByIdWithDetailsAsync(stub.ProjectId, ct)
                      ?? throw new KeyNotFoundException("Projeto nao encontrado.");

        if (project.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Apenas o dono do projeto pode aceitar propostas.");

        if (project.Status != ProjectStatus.Open)
            throw new InvalidOperationException("Este projeto nao esta mais em aberto.");

        var targetProposal = project.Proposals.FirstOrDefault(p => p.Id == proposalId)
                             ?? throw new KeyNotFoundException("Proposta nao pertence a este projeto.");

        foreach (var p in project.Proposals)
            p.Status = p.Id == proposalId ? ProposalStatus.Accepted : ProposalStatus.Rejected;

        project.Status        = ProjectStatus.InProgress;
        project.AcceptedAt    = DateTime.UtcNow;
        project.AcceptedDevId = targetProposal.DevId;

        await projectRepo.SaveChangesAsync(ct);

        string devName = targetProposal.Dev?.Name ?? "o desenvolvedor";

        await notifRepo.AddAsync(new Notification
        {
            UserId    = targetProposal.DevId,
            Message   = $"Sua proposta para \"{project.Title}\" foi aceita! \U0001f389",
            ProjectId = project.Id
        }, ct);

        await notifRepo.AddAsync(new Notification
        {
            UserId    = requesterId,
            Message   = $"Voce aceitou a proposta de {devName}. Projeto em andamento!",
            ProjectId = project.Id
        }, ct);

        await notifRepo.SaveChangesAsync(ct);

        await cache.RemoveAsync($"project_{project.Id}", ct);
        await cache.RemoveByPrefixAsync("projects_list", ct);
    }

    private static ProposalResponse MapToResponse(Proposal p, bool includeProjectTitle = false) => new()
    {
        Id           = p.Id,
        ProjectId    = p.ProjectId,
        ProjectTitle = includeProjectTitle ? p.Project?.Title : null,
        DevId        = p.DevId,
        DevName      = p.Dev?.Name  ?? string.Empty,
        DevEmail     = p.Dev?.Email ?? string.Empty,
        Value        = p.Value,
        Deadline     = p.Deadline,
        Message      = p.Message,
        Status       = p.Status.ToString(),
        CreatedAt    = p.CreatedAt
    };
}

