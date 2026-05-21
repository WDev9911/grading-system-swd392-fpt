namespace SWD392.GradingTool.Application.DTOs;

public class CreateStudentRequest
{
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int ClassGroupId { get; set; }
}