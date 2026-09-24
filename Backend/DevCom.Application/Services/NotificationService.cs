using DevCom.Application.DTOs.Notifications;
using DevCom.Application.Interfaces;

namespace DevCom.Application.Services;

public class NotificationService(INotificationRepository notifRepo)
{
    public async Task<List<NotificationResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        var list = await notifRepo.GetByUserAsync(userId, ct);
        return list.Select(n => new NotificationResponse
        {
            Id        = n.Id,
            Message   = n.Message,
            IsRead    = n.IsRead,
            CreatedAt = n.CreatedAt,
            ProjectId = n.ProjectId
        }).ToList();
    }

    public async Task<UnreadCountResponse> GetUnreadCountAsync(int userId, CancellationToken ct = default)
    {
        int count = await notifRepo.CountUnreadAsync(userId, ct);
        return new UnreadCountResponse { Count = count };
    }

    public Task MarkAllReadAsync(int userId, CancellationToken ct = default) =>
        notifRepo.MarkAllReadAsync(userId, ct);
}
