using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Domain.Enums;
using DevCom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCom.Infrastructure.Repositories;

public class ProjectRepository(AppDbContext db) : IProjectRepository
{
    public async Task<(IReadOnlyList<Project> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        ProjectStatus? status, string? category, string? search,
        CancellationToken ct = default)
    {
        var query = db.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tags)
            .Include(p => p.Proposals)
            .Include(p => p.AcceptedDev)
            .AsNoTracking();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var safe = search.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
            query = query.Where(p =>
                EF.Functions.ILike(p.Title, $"%{safe}%", "\\") ||
                EF.Functions.ILike(p.Description, $"%{safe}%", "\\"));
        }

        int total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items.AsReadOnly(), total);
    }

    public Task<List<Project>> GetByOwnerAsync(int ownerId, CancellationToken ct = default) =>
        db.Projects
          .Include(p => p.Tags)
          .Include(p => p.Proposals)
          .Include(p => p.AcceptedDev)
          .Where(p => p.OwnerId == ownerId)
          .OrderByDescending(p => p.CreatedAt)
          .ToListAsync(ct);

    public Task<List<Project>> GetCompletedByDevAsync(int devId, CancellationToken ct = default) =>
        db.Projects
          .Include(p => p.Tags)
          .Where(p => p.AcceptedDevId == devId && p.Status == ProjectStatus.Completed)
          .OrderByDescending(p => p.AcceptedAt)
          .ToListAsync(ct);

    public Task<Project?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default) =>
        db.Projects
          .Include(p => p.Owner)
          .Include(p => p.Tags)
          .Include(p => p.AcceptedDev)
          .Include(p => p.Proposals).ThenInclude(pr => pr.Dev)
          .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Project project, CancellationToken ct = default) =>
        await db.Projects.AddAsync(project, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}