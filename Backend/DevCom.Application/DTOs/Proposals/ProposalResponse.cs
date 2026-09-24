namespace DevCom.Application.DTOs.Proposals;

public class ProposalResponse
{
    public int Id { get; init; }
    public int ProjectId { get; init; }
    public string? ProjectTitle { get; init; }
    public int DevId { get; init; }
    public string DevName { get; init; } = string.Empty;
    public string DevEmail { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Deadline { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
