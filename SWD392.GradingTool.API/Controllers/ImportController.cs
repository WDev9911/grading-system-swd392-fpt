using Microsoft.AspNetCore.Mvc;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.API.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController : ControllerBase
{
    private readonly IImportStudentService _importStudentService;
    private readonly IImportRubricService _importRubricService;

    public ImportController(
        IImportStudentService importStudentService,
        IImportRubricService importRubricService)
    {
        _importStudentService = importStudentService;
        _importRubricService = importRubricService;
    }

    /// <summary>
    /// Import danh sách sinh viên từ file .txt
    /// </summary>
    /// <remarks>
    /// Format file TXT: mỗi dòng theo dạng "StudentCode, FullName".
    /// Tên file sẽ được dùng làm tên lớp (ví dụ: SE1701.txt → ClassName = SE1701).
    /// </remarks>
    [HttpPost("students")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportStudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ImportStudents(
        [FromForm] ImportStudentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _importStudentService.ImportAsync(request.File, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Import danh sách tiêu chí chấm điểm từ file .txt
    /// </summary>
    /// <remarks>
    /// Format file TXT: mỗi dòng theo dạng "RubricName|MaxScore".
    /// Ví dụ: "ERD|2"
    /// </remarks>
    [HttpPost("rubrics")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportRubricResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ImportRubrics(
        [FromForm] ImportRubricRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _importRubricService.ImportAsync(request.File, cancellationToken);
        return Ok(result);
    }
}
