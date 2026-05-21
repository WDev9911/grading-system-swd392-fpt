namespace SWD392.GradingTool.Domain.Entities;

public class ClassGroup
{
    public int Id { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
