using DevCom.Domain.Entities;

namespace DevCom.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserAsync(int userId, CancellationToken ct = default);
    Task<int> CountUnreadAsync(int userId, CancellationToken ct = default);
    Task MarkAllReadAsync(int userId, CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
