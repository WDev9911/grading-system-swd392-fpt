namespace SWD392.GradingTool.Application.DTOs;

public class ImportStudentDataDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public List<StudentDto> Students { get; set; } = new();
}

public class ImportStudentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ImportStudentDataDto? Data { get; set; }
}
