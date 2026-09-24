using DevCom.Domain.Enums;

namespace DevCom.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<Proposal> SentProposals { get; set; } = new List<Proposal>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Feedback> ReceivedFeedbacks { get; set; } = new List<Feedback>();
    public ICollection<DevSkill> Skills { get; set; } = new List<DevSkill>();
}
