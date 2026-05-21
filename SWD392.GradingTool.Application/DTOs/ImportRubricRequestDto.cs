using Microsoft.AspNetCore.Http;

namespace SWD392.GradingTool.Application.DTOs;

/// <summary>
/// Wrapper request DTO cho file upload rubric – Swashbuckle yêu cầu IFormFile
/// phải nằm trong một class khi dùng [FromForm]
/// </summary>
public class ImportRubricRequestDto
{
    public IFormFile File { get; set; } = null!;
}
