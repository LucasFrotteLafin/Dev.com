using DevCom.Domain.Entities;
using DevCom.Domain.Enums;

namespace DevCom.Application.Interfaces;

public interface IProjectRepository
{
    Task<(IReadOnlyList<Project> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        ProjectStatus? status, string? category, string? search,
        CancellationToken ct = default);

    Task<List<Project>> GetByOwnerAsync(int ownerId, CancellationToken ct = default);
    Task<List<Project>> GetCompletedByDevAsync(int devId, CancellationToken ct = default);
    Task<Project?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
