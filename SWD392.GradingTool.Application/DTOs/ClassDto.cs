namespace SWD392.GradingTool.Application.DTOs;

public class ClassDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public DateTime CreatedDate { get; set; }
}
