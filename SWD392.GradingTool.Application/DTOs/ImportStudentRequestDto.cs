using Microsoft.AspNetCore.Http;



/// <summary>
/// Wrapper request DTO cho file upload – Swashbuckle yêu cầu IFormFile
/// phải nằm trong một class khi dùng [FromForm]
/// </summary>
public class ImportStudentRequestDto
{
    public IFormFile File { get; set; } = null!;
}
