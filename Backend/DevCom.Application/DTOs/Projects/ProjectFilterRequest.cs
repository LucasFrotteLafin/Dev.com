namespace DevCom.Application.DTOs.Projects;

public class ProjectFilterRequest
{
    public string? Status   { get; set; }
    public string? Category { get; set; }
    public string? Search   { get; set; }
    public int     Page     { get; set; } = 1;
    public int     PageSize { get; set; } = 10;
}
