namespace DevCom.Domain.Entities;

public class Feedback
{
    public int Id { get; set; }
    public int Stars { get; set; }   // 1–5
    public string Text { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int DevId { get; set; }
    public User Dev { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
