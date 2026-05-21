namespace SWD392.GradingTool.Application.DTOs;

public class RubricDto
{
    public int RubricId { get; set; }
    public string RubricName { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public string? Description { get; set; }
}
