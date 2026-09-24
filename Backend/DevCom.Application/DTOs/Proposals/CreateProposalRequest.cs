using System.ComponentModel.DataAnnotations;

namespace DevCom.Application.DTOs.Proposals;

public class CreateProposalRequest
{
    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public string Deadline { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Message { get; set; } = string.Empty;
}
