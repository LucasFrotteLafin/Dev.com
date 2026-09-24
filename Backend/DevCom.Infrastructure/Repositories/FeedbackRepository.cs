using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCom.Infrastructure.Repositories;

public class FeedbackRepository(AppDbContext db) : IFeedbackRepository
{
    public Task<List<Feedback>> GetByDevAsync(int devId, CancellationToken ct = default) =>
        db.Feedbacks
          .Include(f => f.Project)
          .Where(f => f.DevId == devId)
          .OrderByDescending(f => f.CreatedAt)
          .ToListAsync(ct);
}
