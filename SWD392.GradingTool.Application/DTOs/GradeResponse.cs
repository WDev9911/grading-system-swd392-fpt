namespace SWD392.GradingTool.Application.DTOs;

public class GradeItemResponse
{
    public int RubricId { get; set; }
    public string RubricName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string? Comment { get; set; }
}

public class GradeResponse
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public decimal TotalScore { get; set; }
    public List<GradeItemResponse> Grades { get; set; } = new();
}
