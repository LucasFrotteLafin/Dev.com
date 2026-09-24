using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCom.Infrastructure.Repositories;

public class NotificationRepository(AppDbContext db) : INotificationRepository
{
    public Task<List<Notification>> GetByUserAsync(int userId, CancellationToken ct = default) =>
        db.Notifications
          .Where(n => n.UserId == userId)
          .OrderByDescending(n => n.CreatedAt)
          .ToListAsync(ct);

    public Task<int> CountUnreadAsync(int userId, CancellationToken ct = default) =>
        db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public async Task MarkAllReadAsync(int userId, CancellationToken ct = default)
    {
        await db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
    }

    public async Task AddAsync(Notification notification, CancellationToken ct = default) =>
        await db.Notifications.AddAsync(notification, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
