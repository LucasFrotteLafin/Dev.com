using System.ComponentModel.DataAnnotations;

namespace DevCom.Application.DTOs.Projects;

public class CreateProjectRequest
{
    [Required, MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(1, double.MaxValue)]
    public decimal Budget { get; set; }

    [Required]
    public string Deadline { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();
}
