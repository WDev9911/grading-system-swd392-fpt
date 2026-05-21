namespace SWD392.GradingTool.Application.DTOs;

public class GradeDetailItem
{
    public int RubricId { get; set; }
    public string RubricName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string? Comment { get; set; }
}

public class StudentGradeDetailResponse
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal TotalScore { get; set; }
    public List<GradeDetailItem> Grades { get; set; } = new();
}