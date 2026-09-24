namespace DevCom.Application.DTOs.Devs;

public class DevProfileResponse
{
    public string Name { get; init; } = string.Empty;
    public string? Bio { get; init; }
    public double Rating { get; init; }
    public DateTime MemberSince { get; init; }
    public int CompletedCount { get; init; }
    public List<string> Skills { get; init; } = new();
    public List<PastProjectDto> PastProjects { get; init; } = new();
    public List<FeedbackDto> Feedbacks { get; init; } = new();
}

public class PastProjectDto
{
    public string Title { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
    public string Duration { get; init; } = string.Empty;
}

public class FeedbackDto
{
    public int Stars { get; init; }
    public string Text { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
}
