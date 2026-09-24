using DevCom.Domain.Enums;

namespace DevCom.Domain.Entities;

public class Proposal
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;    // texto livre: "R$ 2.500"
    public string Deadline { get; set; } = string.Empty; // texto livre: "20 dias"
    public string Message { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int DevId { get; set; }
    public User Dev { get; set; } = null!;
}
