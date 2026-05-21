namespace SWD392.GradingTool.Application.DTOs;

public class StudentGradeResponse
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public decimal TotalScore { get; set; }
    public List<GradeItemResponse> Grades { get; set; } = new();
}

public class ClassGradeResponse
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<StudentGradeResponse> Students { get; set; } = new();
}
