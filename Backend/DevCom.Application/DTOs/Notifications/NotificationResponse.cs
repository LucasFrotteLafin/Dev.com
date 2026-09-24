namespace DevCom.Application.DTOs.Notifications;

public class NotificationResponse
{
    public int Id { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? ProjectId { get; init; }
}

public class UnreadCountResponse
{
    public int Count { get; init; }
}
