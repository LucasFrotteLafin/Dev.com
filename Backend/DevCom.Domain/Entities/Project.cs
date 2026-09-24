using DevCom.Domain.Enums;

namespace DevCom.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public string Deadline { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public int? AcceptedDevId { get; set; }
    public User? AcceptedDev { get; set; }

    public ICollection<ProjectTag> Tags { get; set; } = new List<ProjectTag>();
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}
