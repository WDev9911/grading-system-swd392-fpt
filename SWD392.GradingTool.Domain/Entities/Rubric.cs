namespace SWD392.GradingTool.Domain.Entities;

public class Rubric
{
    public int Id { get; set; }
    public string RubricName { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
