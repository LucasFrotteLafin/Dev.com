using DevCom.Domain.Entities;

namespace DevCom.Application.Interfaces;

public interface IProposalRepository
{
    Task<List<Proposal>> GetByProjectAsync(int projectId, CancellationToken ct = default);
    Task<List<Proposal>> GetByDevAsync(int devId, CancellationToken ct = default);
    Task<Proposal?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int projectId, int devId, CancellationToken ct = default);
    Task AddAsync(Proposal proposal, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
