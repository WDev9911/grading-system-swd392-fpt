namespace SWD392.GradingTool.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    // Foreign key
    public int ClassGroupId { get; set; }

    // Navigation property
    public ClassGroup ClassGroup { get; set; } = null!;
}
