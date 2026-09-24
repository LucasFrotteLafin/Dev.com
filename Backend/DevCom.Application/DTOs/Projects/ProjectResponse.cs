namespace DevCom.Application.DTOs.Projects;

public class ProjectResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Budget { get; init; }
    public string Deadline { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public int ProposalCount { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? AcceptedAt { get; init; }
    public string? AcceptedDevName { get; init; }
}
