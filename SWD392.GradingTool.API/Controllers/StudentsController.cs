using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IValidator<CreateStudentRequest> _createValidator;

    public StudentsController(
        IStudentService studentService,
        IValidator<CreateStudentRequest> createValidator)
    {
        _studentService = studentService;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Tìm kiếm sinh viên theo Roll number (StudentCode) hoặc FullName.
    /// </summary>
    /// <param name="keyword">Từ khoá tìm kiếm — match một phần với StudentCode hoặc FullName.</param>
    /// <returns>Danh sách sinh viên khớp với keyword, kèm theo ClassName.</returns>
    /// <remarks>
    /// Ví dụ: GET /api/students/search?keyword=SE12345
    /// </remarks>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string keyword, CancellationToken cancellationToken)
    {
        var students = await _studentService.SearchAsync(keyword, cancellationToken);

        var response = new
        {
            Success = true,
            Message = "Search students successfully",
            Data = students
        };

        return Ok(response);
    }

    /// <summary>
    /// Thêm thủ công một sinh viên vào một lớp đã có sẵn.
    /// </summary>
    /// <remarks>
    /// Bổ trợ cho luồng import file — dùng khi teacher muốn thêm lẻ tẻ một sinh viên
    /// trực tiếp từ form trên UI. ClassGroupId phải là Id của lớp đã tồn tại trong hệ thống.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        // 1. Validate bằng FluentValidation
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        // 2. Gọi service xử lý
        var created = await _studentService.CreateAsync(request, cancellationToken);

        // 3. Trả về 201 Created
        var response = new
        {
            Success = true,
            Message = "Create student successfully",
            Data = created
        };

        return CreatedAtAction(nameof(Search), response);
    }
}