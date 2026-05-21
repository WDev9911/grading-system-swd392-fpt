namespace SWD392.GradingTool.Application.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int ClassId { get; set; }
}
