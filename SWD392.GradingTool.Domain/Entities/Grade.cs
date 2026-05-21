namespace SWD392.GradingTool.Domain.Entities;

public class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int RubricId { get; set; }
    public decimal Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Rubric Rubric { get; set; } = null!;
}
