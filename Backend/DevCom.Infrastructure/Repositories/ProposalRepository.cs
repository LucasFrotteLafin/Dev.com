using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCom.Infrastructure.Repositories;

public class ProposalRepository(AppDbContext db) : IProposalRepository
{
    public Task<List<Proposal>> GetByProjectAsync(int projectId, CancellationToken ct = default) =>
        db.Proposals
          .Include(p => p.Dev)
          .Include(p => p.Project)
          .Where(p => p.ProjectId == projectId)
          .OrderByDescending(p => p.CreatedAt)
          .ToListAsync(ct);

    public Task<List<Proposal>> GetByDevAsync(int devId, CancellationToken ct = default) =>
        db.Proposals
          .Include(p => p.Project)
          .Include(p => p.Dev)
          .Where(p => p.DevId == devId)
          .OrderByDescending(p => p.CreatedAt)
          .ToListAsync(ct);

    public Task<Proposal?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Proposals
          .Include(p => p.Dev)
          .Include(p => p.Project)
          .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<bool> ExistsAsync(int projectId, int devId, CancellationToken ct = default) =>
        db.Proposals.AnyAsync(p => p.ProjectId == projectId && p.DevId == devId, ct);

    public async Task AddAsync(Proposal proposal, CancellationToken ct = default) =>
        await db.Proposals.AddAsync(proposal, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
