using DevCom.Application.DTOs.Common;
using DevCom.Application.DTOs.Projects;
using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Domain.Enums;

namespace DevCom.Application.Services;

public class ProjectService(
    IProjectRepository projectRepo,
    IUserRepository    userRepo,
    ICacheService      cache)
{
    private const string ListCachePrefix = "projects_list";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<PagedResult<ProjectResponse>> GetPagedAsync(
        ProjectFilterRequest filter, CancellationToken ct = default)
    {
        if (filter.Page < 1)      filter.Page     = 1;
        if (filter.PageSize < 1)  filter.PageSize = 10;
        if (filter.PageSize > 50) filter.PageSize = 50;

        var cacheKey = BuildListCacheKey(filter);

        var cached = await cache.GetAsync<PagedResult<ProjectResponse>>(cacheKey, ct);
        if (cached is not null)
            return cached;

        ProjectStatus? status = filter.Status is not null
            && Enum.TryParse<ProjectStatus>(filter.Status, out var s) ? s : null;

        var (items, total) = await projectRepo.GetPagedAsync(
            filter.Page, filter.PageSize, status, filter.Category, filter.Search, ct);

        var result = PagedResult<ProjectResponse>.Create(
            items.Select(MapToResponse).ToList().AsReadOnly(),
            total, filter.Page, filter.PageSize);

        await cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<List<ProjectResponse>> GetMyProjectsAsync(int ownerId, CancellationToken ct = default)
    {
        var projects = await projectRepo.GetByOwnerAsync(ownerId, ct);
        return projects.Select(MapToResponse).ToList();
    }

    public async Task<ProjectResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var cacheKey = $"project_{id}";
        var cached   = await cache.GetAsync<ProjectResponse>(cacheKey, ct);
        if (cached is not null) return cached;

        var project = await projectRepo.GetByIdWithDetailsAsync(id, ct)
                      ?? throw new KeyNotFoundException("Projeto não encontrado.");

        var response = MapToResponse(project);
        await cache.SetAsync(cacheKey, response, CacheTtl, ct);
        return response;
    }

    public async Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request, int ownerId, CancellationToken ct = default)
    {
        var owner = await userRepo.GetByIdAsync(ownerId, ct)
                    ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (owner.Role != UserRole.Client)
            throw new UnauthorizedAccessException("Apenas clientes podem publicar projetos.");

        var project = new Project
        {
            Title       = request.Title.Trim(),
            Description = request.Description.Trim(),
            Budget      = request.Budget,
            Deadline    = request.Deadline.Trim(),
            Category    = request.Category.Trim(),
            OwnerId     = ownerId,
            Tags        = request.Tags.Select(t => new ProjectTag { Name = t.Trim() }).ToList()
        };

        await projectRepo.AddAsync(project, ct);
        await projectRepo.SaveChangesAsync(ct);

        // Invalida todo o cache de listagem ao criar projeto
        await cache.RemoveByPrefixAsync(ListCachePrefix, ct);

        project.Owner = owner;
        return MapToResponse(project);
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private static string BuildListCacheKey(ProjectFilterRequest f) =>
        $"{ListCachePrefix}_p{f.Page}_s{f.PageSize}_st{f.Status}_cat{f.Category}_q{f.Search}";

    private static ProjectResponse MapToResponse(Project p) => new()
    {
        Id              = p.Id,
        Title           = p.Title,
        Description     = p.Description,
        Budget          = p.Budget,
        Deadline        = p.Deadline,
        Category        = p.Category,
        Status          = p.Status.ToString(),
        OwnerId         = p.OwnerId,
        OwnerName       = p.Owner?.Name ?? string.Empty,
        ProposalCount   = p.Proposals?.Count ?? 0,
        Tags            = p.Tags?.Select(t => t.Name).ToList() ?? new(),
        CreatedAt       = p.CreatedAt,
        AcceptedAt      = p.AcceptedAt,
        AcceptedDevName = p.AcceptedDev?.Name
    };
}
