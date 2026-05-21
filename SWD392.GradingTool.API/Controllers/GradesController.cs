using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.API.Controllers;

[ApiController]
[Route("api/grades")]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IValidator<GradeStudentRequest> _validator;

    public GradesController(IGradeService gradeService, IValidator<GradeStudentRequest> validator)
    {
        _gradeService = gradeService;
        _validator = validator;
    }

    /// <summary>
    /// Nhập điểm cho sinh viên
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GradeStudent([FromBody] GradeStudentRequest request, CancellationToken cancellationToken)
    {
        // 1. Validate bằng FluentValidation
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        // 2. Gọi service xử lý logic nhập điểm
        var responseDto = await _gradeService.GradeStudentAsync(request, cancellationToken);

        // 3. Trả về kết quả
        var response = new
        {
            Success = true,
            Message = "Grade student successfully",
            Data = responseDto
        };

        return Ok(response);
    }

    /// <summary>
    /// Lấy bảng điểm của một lớp
    /// </summary>
    [HttpGet("class/{classId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGradesByClass(int classId, CancellationToken cancellationToken)
    {
        if (classId <= 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid ClassId" });
        }

        var responseDto = await _gradeService.GetGradesByClassAsync(classId, cancellationToken);

        var response = new
        {
            Success = true,
            Message = "Get grades successfully",
            Data = responseDto
        };

        return Ok(response);
    }

    /// <summary>
    /// Lấy chi tiết grading của một sinh viên (rubric, score, comment, total score).
    /// </summary>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentGradeDetails(int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid StudentId",
                Detail = "StudentId phải lớn hơn 0."
            });
        }

        var responseDto = await _gradeService.GetStudentGradeDetailsAsync(studentId, cancellationToken);

        var response = new
        {
            Success = true,
            Message = "Get student grade details successfully",
            Data = responseDto
        };

        return Ok(response);
    }

    /// <summary>
    /// Export bảng điểm của một lớp ra file Excel (.xlsx).
    /// </summary>
    /// <remarks>
    /// Response là file Excel binary (download trực tiếp), không phải JSON wrapper.
    /// Format: STT | StudentCode | FullName | &lt;Rubric1 (Max)&gt; | ... | Total | Comment.
    /// Cột Total dùng công thức SUM nên teacher edit điểm trong Excel thì tự cập nhật.
    /// </remarks>
    [HttpGet("class/{classId}/export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportClassGrades(int classId, CancellationToken cancellationToken)
    {
        if (classId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid ClassId",
                Detail = "ClassId phải lớn hơn 0."
            });
        }

        var result = await _gradeService.ExportClassGradesAsync(classId, cancellationToken);
        return File(result.Content, result.ContentType, result.FileName);
    }
}