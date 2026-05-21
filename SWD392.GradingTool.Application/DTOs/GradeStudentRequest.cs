namespace SWD392.GradingTool.Application.DTOs;

public class GradeItemRequest
{
    public int RubricId { get; set; }
    public decimal Score { get; set; }
    public string? Comment { get; set; }
}

public class GradeStudentRequest
{
    public int StudentId { get; set; }
    public List<GradeItemRequest> Grades { get; set; } = new();
}
