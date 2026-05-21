namespace SWD392.GradingTool.Application.DTOs;

public class ImportRubricDataDto
{
    public int TotalRubrics { get; set; }
    public List<RubricDto> Rubrics { get; set; } = new();
}

public class ImportRubricResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ImportRubricDataDto? Data { get; set; }
}
